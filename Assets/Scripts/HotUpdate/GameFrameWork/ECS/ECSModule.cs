using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;


public class ECSModule : BaseGameModule
{
    public ECSWorld World { get; private set; }

    private Dictionary<Type, IAwakeSystem> awakeSystemMap; //��ϵͳ����ӳ�䵽 IAwakeSystem �ӿڵ�ʵ��
    private Dictionary<Type, IDestroySystem> destroySystemMap;//������ʵ���ϵͳ����ʱִ�в����� IDestroySystem

    private Dictionary<Type, IUpdateSystem> updateSystemMap;//ÿ֡�����µ�ϵͳ
    private Dictionary<IUpdateSystem, List<ECSEntity>> updateSystemRelatedEntityMap;//��IUpdateSystem ʵ��ӳ�䵽��������ص� ECSEntity �б���ֵ�

    private Dictionary<Type, ILateUpdateSystem> lateUpdateSystemMap;//lateUpdate���µ�ϵͳ
    private Dictionary<ILateUpdateSystem, List<ECSEntity>> lateUpdateSystemRelatedEntityMap;

    private Dictionary<Type, IFixedUpdateSystem> fixedUpdateSystemMap;////lateUpdate���µ�ϵͳ
    private Dictionary<IFixedUpdateSystem, List<ECSEntity>> fixedUpdateSystemRelatedEntityMap;

    private Dictionary<long, ECSEntity> entities = new Dictionary<long, ECSEntity>();//�洢ʵ��
    private Dictionary<Type, List<IEntityMessageHandler>> entityMessageHandlerMap;//������Ϣ
    private Dictionary<Type, IEntityRpcHandler> entityRpcHandlerMap;//Զ�̹��̵��ã�RPC���Ĵ�����


    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();
        LoadAllSystems();
        World = new ECSWorld();
    }

    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);
        DriveUpdateSystem();
        
    }

    protected internal override void OnModuleLateUpdate(float deltaTime)
    {
        base.OnModuleLateUpdate(deltaTime);
        DriveLateUpdateSystem();
    }

    protected internal override void OnModuleFixedUpdate(float deltaTime)
    {
        base.OnModuleFixedUpdate(deltaTime);
        DriveFixedUpdateSystem();
    }

    /// <summary>
    /// ģ���ʼ��ʱ����
    /// </summary>
    public void LoadAllSystems()
    {
        awakeSystemMap = new Dictionary<Type, IAwakeSystem>();
        destroySystemMap = new Dictionary<Type, IDestroySystem>();

        updateSystemMap = new Dictionary<Type, IUpdateSystem>();
        updateSystemRelatedEntityMap = new Dictionary<IUpdateSystem, List<ECSEntity>>();

        lateUpdateSystemMap = new Dictionary<Type, ILateUpdateSystem>();
        lateUpdateSystemRelatedEntityMap = new Dictionary<ILateUpdateSystem, List<ECSEntity>>();

        fixedUpdateSystemMap = new Dictionary<Type, IFixedUpdateSystem>();
        fixedUpdateSystemRelatedEntityMap = new Dictionary<IFixedUpdateSystem, List<ECSEntity>>();

        entityMessageHandlerMap = new Dictionary<Type, List<IEntityMessageHandler>>();
        entityRpcHandlerMap = new Dictionary<Type, IEntityRpcHandler>();

        //��ȡ��ǰ���ó����е���������
        foreach (var type in Assembly.GetCallingAssembly().GetTypes())
        {
            //�Ƿ��ǳ���ģ��������������
            if (type.IsAbstract)
                continue;

            //�Ƿ����`ECSSystemAttribute`�Զ�������
            if (type.GetCustomAttribute<ECSSystemAttribute>(true) != null)
            {
                // AwakeSystem 
                Type awakeSystemType = typeof(IAwakeSystem);
                  //��������Ƿ����ʵ�ָýӿ�;
                if (awakeSystemType.IsAssignableFrom(type))
                {
                    //����Ѿ���һ����ͬ���͵�ϵͳ�����ڶ�Ӧ��ӳ���ֵ��У����¼һ����������������
                    if (awakeSystemMap.ContainsKey(type))
                    {
                        UnityLog.Error($"Duplicated Awake System:{type.FullName}");
                        continue;
                    }

                    //���û���ظ���ʹ��`Activator.CreateInstance`���������͵�һ��ʵ��
                    IAwakeSystem awakeSystem = Activator.CreateInstance(type) as IAwakeSystem;
                    //������ӵ���Ӧ��ӳ���ֵ���
                    awakeSystemMap.Add(type, awakeSystem);
                }

                // DestroySystem  ��ȡ�ӿ�����
                Type destroySystemType = typeof(IDestroySystem);
                //type�Ƿ�ʵ����IDestroySystem�ӿ�
                if (destroySystemType.IsAssignableFrom(type))
                {
                    //�Ƿ����
                    if (destroySystemMap.ContainsKey(type))
                    {
                        UnityLog.Error($"Duplicated Destroy System:{type.FullName}");
                        continue;
                    }

                    //����type��һ����ʵ����������ת��ΪIDestroySystem�ӿ�����
                    IDestroySystem destroySytem = Activator.CreateInstance(type) as IDestroySystem;
                    destroySystemMap.Add(type, destroySytem);
                }

                // UpdateSystem
                Type updateSystemType = typeof(IUpdateSystem);
                if (updateSystemType.IsAssignableFrom(type))
                {
                    if (updateSystemMap.ContainsKey(type))
                    {
                        UnityLog.Error($"Duplicated Update System:{type.FullName}");
                        continue;
                    }

                    IUpdateSystem updateSystem = Activator.CreateInstance(type) as IUpdateSystem;
                    updateSystemMap.Add(type, updateSystem);

                    updateSystemRelatedEntityMap.Add(updateSystem, new List<ECSEntity>());
                }

                // LateUpdateSystem
                Type lateUpdateSystemType = typeof(ILateUpdateSystem);
                if (lateUpdateSystemType.IsAssignableFrom(type))
                {
                    if (lateUpdateSystemMap.ContainsKey(type))
                    {
                        UnityLog.Error($"Duplicated Late update System:{type.FullName}");
                        continue;
                    }

                    ILateUpdateSystem lateUpdateSystem = Activator.CreateInstance(type) as ILateUpdateSystem;
                    lateUpdateSystemMap.Add(type, lateUpdateSystem);

                    lateUpdateSystemRelatedEntityMap.Add(lateUpdateSystem, new List<ECSEntity>());
                }

                // FixedUpdateSystem
                Type fixedUpdateSystemType = typeof(IFixedUpdateSystem);
                if (fixedUpdateSystemType.IsAssignableFrom(type))
                {
                    if (fixedUpdateSystemMap.ContainsKey(type))
                    {
                        UnityLog.Error($"Duplicated Late update System:{type.FullName}");
                        continue;
                    }

                    IFixedUpdateSystem fixedUpdateSystem = Activator.CreateInstance(type) as IFixedUpdateSystem;
                    fixedUpdateSystemMap.Add(type, fixedUpdateSystem);

                    fixedUpdateSystemRelatedEntityMap.Add(fixedUpdateSystem, new List<ECSEntity>());
                }
            }

            //���������е���������
            if (type.GetCustomAttribute<EntityMessageHandlerAttribute>(true) != null)
            {
                // EntityMessage
                Type entityMessageType = typeof(IEntityMessageHandler);
                if (entityMessageType.IsAssignableFrom(type))
                {
                    IEntityMessageHandler entityMessageHandler = Activator.CreateInstance(type) as IEntityMessageHandler;

                    if (!entityMessageHandlerMap.TryGetValue(entityMessageHandler.MessageType(), out List<IEntityMessageHandler> list))
                    {
                        list = new List<IEntityMessageHandler>();
                        entityMessageHandlerMap.Add(entityMessageHandler.MessageType(), list);
                    }

                    list.Add(entityMessageHandler);
                }
            }

            if (type.GetCustomAttribute<EntityRpcHandlerAttribute>(true) != null)
            {
                // EntityRPC
                Type entityMessageType = typeof(IEntityRpcHandler);
                if (entityMessageType.IsAssignableFrom(type))
                {
                    IEntityRpcHandler entityRpcHandler = Activator.CreateInstance(type) as IEntityRpcHandler;

                    if (entityRpcHandlerMap.ContainsKey(entityRpcHandler.RpcType()))
                    {
                        UnityLog.Error($"Duplicate Entity Rpc, type:{entityRpcHandler.RpcType().FullName}");
                        continue;
                    }

                    entityRpcHandlerMap.Add(entityRpcHandler.RpcType(), entityRpcHandler);
                }
            }
        }
    }

    /// <summary>
    /// ����ϵͳ
    /// </summary>
    private void DriveUpdateSystem()
    {
        //�������и���ϵͳ
        foreach (IUpdateSystem updateSystem in updateSystemMap.Values)
        {
            //�õ��б�
            List<ECSEntity> updateSystemRelatedEntities = updateSystemRelatedEntityMap[updateSystem];
            if (updateSystemRelatedEntities.Count == 0)
                continue;

            //������л�ȡһ���µ��б�
            List<ECSEntity> entityList = ListPool<ECSEntity>.Obtain();
            //ֱ�ӽ��б����
            entityList.AddRangeNonAlloc(updateSystemRelatedEntities);
            foreach (var entity in entityList)
            {
                //��ǰʵ��û�б��۲� ����
                if (!updateSystem.ObservingEntity(entity))
                    continue;

                //����ʵ��
                updateSystem.Update(entity);
            }

            //�ͷŶ���ض����
            ListPool<ECSEntity>.Release(entityList);
        }
    }

    /// <summary>
    /// ��������ע���� IUpdateSystem �ӿڵ�ϵͳ
    /// </summary>
    private void DriveLateUpdateSystem()
    {
        //�����ֵ�
        foreach (ILateUpdateSystem lateUpdateSystem in lateUpdateSystemMap.Values)
        {
            //��ȡ�����ϵͳ��ص�ʵ��
            List<ECSEntity> lateUpdateSystemRelatedEntities = lateUpdateSystemRelatedEntityMap[lateUpdateSystem];
            //���ʵ���б��Ƿ�Ϊ��
            if (lateUpdateSystemRelatedEntities.Count == 0)
                continue;

            //������л�ȡ�µ��б�
            List<ECSEntity> entityList = ListPool<ECSEntity>.Obtain();
            //�б����
            entityList.AddRangeNonAlloc(lateUpdateSystemRelatedEntities);
            foreach (var entity in entityList)
            {
                if (!lateUpdateSystem.ObservingEntity(entity))
                    continue;
                //����
                lateUpdateSystem.LateUpdate(entity);
            }

            //����
            ListPool<ECSEntity>.Release(entityList);
        }
    }

    private void DriveFixedUpdateSystem()
    {
        foreach (IFixedUpdateSystem fixedUpdateSystem in fixedUpdateSystemMap.Values)
        {
            List<ECSEntity> fixedUpdateSystemRelatedEntities = fixedUpdateSystemRelatedEntityMap[fixedUpdateSystem];
            if (fixedUpdateSystemRelatedEntities.Count == 0)
                continue;

            List<ECSEntity> entityList = ListPool<ECSEntity>.Obtain();
            entityList.AddRangeNonAlloc(fixedUpdateSystemRelatedEntities);
            foreach (var entity in entityList)
            {
                if (!fixedUpdateSystem.ObservingEntity(entity))
                    continue;

                fixedUpdateSystem.FixedUpdate(entity);
            }

            ListPool<ECSEntity>.Release(entityList);
        }
    }


    /// <summary>
    /// ��ȡAwakeSystems
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <param name="list"></param>
    private void GetAwakeSystems<C>(List<IAwakeSystem> list) where C : ECSComponent
    {
        foreach (var awakeSystem in awakeSystemMap.Values)
        {
            if (awakeSystem.ComponentType() == typeof(C))
            {
                list.Add(awakeSystem);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="C">ECSComponent</typeparam>
    /// <param name="component"></param>
    public void AwakeComponent<C>(C component) where C : ECSComponent
    {
        //����ϵͳʵ���б�
        UpdateSystemEntityList(component.Entity);

        //��ȡ�б�
        List<IAwakeSystem> list = ListPool<IAwakeSystem>.Obtain();
        GetAwakeSystems<C>(list);

        //����
        bool found = false;
        foreach (var item in list)
        {
            AwakeSystem<C> awakeSystem = item as AwakeSystem<C>;
            if (awakeSystem == null)
                continue;

            //ִ���߼�
            awakeSystem.Awake(component);
            found = true;
        }

        //�����ͷ�
        ListPool<IAwakeSystem>.Release(list);
        //û�ҵ� ����
        if (!found)
        {
            UnityLog.Warn($"Not found awake system:<{typeof(C).Name}>");
        }
    }

    public void AwakeComponent<C, P1>(C component, P1 p1) where C : ECSComponent
    {
        UpdateSystemEntityList(component.Entity);

        List<IAwakeSystem> list = ListPool<IAwakeSystem>.Obtain();
        TGameFramework.Instance.GetModule<ECSModule>().GetAwakeSystems<C>(list);

        bool found = false;
        foreach (var item in list)
        {
            AwakeSystem<C, P1> awakeSystem = item as AwakeSystem<C, P1>;
            if (awakeSystem == null)
                continue;

            awakeSystem.Awake(component, p1);
            found = true;
        }

        ListPool<IAwakeSystem>.Release(list);
        if (!found)
        {
            UnityLog.Warn($"Not found awake system:<{typeof(C).Name}, {typeof(P1).Name}>");
        }
    }

    public void AwakeComponent<C, P1, P2>(C component, P1 p1, P2 p2) where C : ECSComponent
    {
        UpdateSystemEntityList(component.Entity);

        List<IAwakeSystem> list = ListPool<IAwakeSystem>.Obtain();
        TGameFramework.Instance.GetModule<ECSModule>().GetAwakeSystems<C>(list);

        bool found = false;
        foreach (var item in list)
        {
            AwakeSystem<C, P1, P2> awakeSystem = item as AwakeSystem<C, P1, P2>;
            if (awakeSystem == null)
                continue;

            awakeSystem.Awake(component, p1, p2);
            found = true;
        }

        ListPool<IAwakeSystem>.Release(list);
        if (!found)
        {
            UnityLog.Warn($"Not found awake system:<{typeof(C).Name}, {typeof(P1).Name}, {typeof(P2).Name}>");
        }
    }

    /// <summary>
    /// ��ȡ�����б�
    /// </summary>
    /// <typeparam name="C">����</typeparam>
    /// <param name="list">Ҫ��ӵ��б�</param>
    private void GetDestroySystems<C>(List<IDestroySystem> list) where C : ECSComponent
    {
        foreach (var destroySystem in destroySystemMap.Values)
        {
            if (destroySystem.ComponentType() == typeof(C))
            {
                list.Add(destroySystem);
            }
        }
    }

    /// <summary>
    /// ��ȡ�����б�
    /// </summary>
    /// <param name="componentType"></param>
    /// <param name="list"></param>
    private void GetDestroySystems(Type componentType, List<IDestroySystem> list)
    {
        foreach (var destroySystem in destroySystemMap.Values)
        {
            if (destroySystem.ComponentType() == componentType)
            {
                list.Add(destroySystem);
            }
        }
    }

    /// <summary>
    /// ���ٶ�Ӧ���͵����
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <param name="component"></param>
    public void DestroyComponent<C>(C component) where C : ECSComponent
    {
        UpdateSystemEntityList(component.Entity);

        List<IDestroySystem> list = ListPool<IDestroySystem>.Obtain();
        GetDestroySystems<C>(list);
        foreach (var item in list)
        {
            DestroySystem<C> destroySystem = item as DestroySystem<C>;
            if (destroySystem == null)
                continue;

            destroySystem.Destroy(component);
            component.Disposed = true;
        }

        ListPool<IDestroySystem>.Release(list);
    }

    /// <summary>
    /// ���ٶ�Ӧ���͵����
    /// </summary>
    /// <param name="component"></param>
    public void DestroyComponent(ECSComponent component)
    {
        UpdateSystemEntityList(component.Entity);

        List<IDestroySystem> list = ListPool<IDestroySystem>.Obtain();
        GetDestroySystems(component.GetType(), list);
        foreach (var item in list)
        {
            item.Destroy(component);
            component.Disposed = true;
        }

        ListPool<IDestroySystem>.Release(list);
    }

    /// <summary>
    /// ���ٶ�Ӧ���͵����
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <param name="component"></param>
    /// <param name="p1"></param>
    public void DestroyComponent<C, P1>(C component, P1 p1) where C : ECSComponent
    {
        //����ϵͳʵ���б�
        UpdateSystemEntityList(component.Entity);

        //��ȡ����ϵͳ�б�
        List<IDestroySystem> list = ListPool<IDestroySystem>.Obtain();
        //��Ҫ���ٵ���ӵ��б�
        GetDestroySystems<C>(list);

        //��������
        foreach (var item in list)
        {
            //ת����
            DestroySystem<C, P1> destroySystem = item as DestroySystem<C, P1>;
            if (destroySystem == null)
                continue;

            //ת�ɹ��� �������ٷ���
            destroySystem.Destroy(component, p1);
            //����ֵ�ı�
            component.Disposed = true;
        }

        ListPool<IDestroySystem>.Release(list);
    }

    /// <summary>
    /// ���ٶ�Ӧ���͵����
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <typeparam name="P2"></typeparam>
    /// <param name="component"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public void DestroyComponent<C, P1, P2>(C component, P1 p1, P2 p2) where C : ECSComponent
    {
        UpdateSystemEntityList(component.Entity);

        List<IDestroySystem> list = ListPool<IDestroySystem>.Obtain();
        GetDestroySystems<C>(list);
        foreach (var item in list)
        {
            DestroySystem<C, P1, P2> destroySystem = item as DestroySystem<C, P1, P2>;
            if (destroySystem == null)
                continue;

            destroySystem.Destroy(component, p1, p2);
            component.Disposed = true;
        }

        ListPool<IDestroySystem>.Release(list);
    }

    /// <summary>
    /// ����ʵ���б�
    /// </summary>
    /// <param name="entity"></param>
    private void UpdateSystemEntityList(ECSEntity entity)
    {
        //����
        foreach (IUpdateSystem updateSystem in updateSystemMap.Values)
        {
            // update entity list ��ȡʵ���б�
            List<ECSEntity> entityList = updateSystemRelatedEntityMap[updateSystem];
            //�Ƿ������ʵ��
            if (!entityList.Contains(entity))
            {
                //������ �����ڹ۲� ��ӵ��б�
                if (updateSystem.ObservingEntity(entity))
                {
                    entityList.Add(entity);
                }
            }
            else
            {
                //���� ���ǲ��۲� �Ƴ�
                if (!updateSystem.ObservingEntity(entity))
                {
                    entityList.Remove(entity);
                }
            }
        }

        //������ͬ��
        foreach (ILateUpdateSystem lateUpdateSystem in lateUpdateSystemMap.Values)
        {
            // update entity list
            List<ECSEntity> entityList = lateUpdateSystemRelatedEntityMap[lateUpdateSystem];
            if (!entityList.Contains(entity))
            {
                if (lateUpdateSystem.ObservingEntity(entity))
                {
                    entityList.Add(entity);
                }
            }
            else
            {
                if (!lateUpdateSystem.ObservingEntity(entity))
                {
                    entityList.Remove(entity);
                }
            }
        }

        //������ͬ��
        foreach (IFixedUpdateSystem fixedUpdateSystem in fixedUpdateSystemMap.Values)
        {
            // update entity list
            List<ECSEntity> entityList = fixedUpdateSystemRelatedEntityMap[fixedUpdateSystem];
            if (!entityList.Contains(entity))
            {
                if (fixedUpdateSystem.ObservingEntity(entity))
                {
                    entityList.Add(entity);
                }
            }
            else
            {
                if (!fixedUpdateSystem.ObservingEntity(entity))
                {
                    entityList.Remove(entity);
                }
            }
        }
    }

    /// <summary>
    /// ���ʵ��
    /// </summary>
    /// <param name="entity"></param>
    public void AddEntity(ECSEntity entity)
    {
        entities.Add(entity.InstanceID, entity);
    }

    /// <summary>
    /// �Ƴ�ʵ��
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveEntity(ECSEntity entity)
    {
        if (entity == null)
            return;

        entities.Remove(entity.InstanceID);
        ECSScene scene = entity.Scene;
        scene?.RemoveEntity(entity.InstanceID);
    }

    /// <summary>
    /// ����ʵ��
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ECSEntity FindEntity(long id)
    {
        return FindEntity<ECSEntity>(id);
    }

    /// <summary>
    /// ���ҷ���ʵ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public T FindEntity<T>(long id) where T : ECSEntity
    {
        entities.TryGetValue(id, out ECSEntity entity);
        return entity as T;
    }

    /// <summary>
    /// ����ʵ������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityID"></param>
    /// <returns></returns>
    public T FindComponentOfEntity<T>(long entityID) where T : ECSComponent
    {
        return FindEntity(entityID)?.GetComponent<T>();
    }

    /// <summary>
    /// ��ʵ�巢����Ϣ
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="id"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public async Task SendMessageToEntity<M>(long id, M m)
    {
        //id Ϊ0 ������
        if (id == 0)
            return;

        //����ʵ��
        ECSEntity entity = FindEntity(id);
        if (entity == null)
            return;

        //��ȡ��Ϣ����
        Type messageType = m.GetType();
        //��ȡ��Ϣ�����б�
        if (!entityMessageHandlerMap.TryGetValue(messageType, out List<IEntityMessageHandler> list))
            return;

        //�Ӷ�����л�ȡ
        List<IEntityMessageHandler> entityMessageHandlers = ListPool<IEntityMessageHandler>.Obtain();
        //���
        entityMessageHandlers.AddRangeNonAlloc(list);
        foreach (IEntityMessageHandler<M> handler in entityMessageHandlers)
        {
            //�첽������Ϣ��ʵ��
            await handler.Post(entity, m);
        }

        //�����ͷ�
        ListPool<IEntityMessageHandler>.Release(entityMessageHandlers);
    }

    /// <summary>
    /// ��ָ����ʵ�巢��Զ�̹��̵���
    /// </summary>
    /// <typeparam name="Request"></typeparam>
    /// <typeparam name="Response"></typeparam>
    /// <param name="entityID">ID</param>
    /// <param name="request">����</param>
    /// <returns></returns>
    public async Task<Response> SendRpcToEntity<Request, Response>(long entityID, Request request) where Response : IEntityRpcResponse, new()
    {
        //����һ������ Error ��������Ϊ true ���� Response ����
        if (entityID == 0)
            return new Response() { Error = true };

        //û�ҵ�ʵ��
        ECSEntity entity = FindEntity(entityID);
        if (entity == null)
            return new Response() { Error = true };

        //��ȡ��������
        Type messageType = request.GetType();
        //�ֵ��л�ȡ�����������ƥ���RPC������
        if (!entityRpcHandlerMap.TryGetValue(messageType, out IEntityRpcHandler entityRpcHandler))
            return new Response() { Error = true };

        //���ҵ���RPC������ת��Ϊ IEntityRpcHandler<Request, Response> ����
        IEntityRpcHandler<Request, Response> handler = entityRpcHandler as IEntityRpcHandler<Request, Response>;
        if (handler == null)
            return new Response() { Error = true };

        //ʹ��ת�����RPC��������handler���� Post �����첽����ʵ�巢��RPC���󣬲��ȴ���Ӧ
        return await handler.Post(entity, request);
    }
}

