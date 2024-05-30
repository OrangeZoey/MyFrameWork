using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;


public class ECSModule : BaseGameModule
{
    public ECSWorld World { get; private set; }

    private Dictionary<Type, IAwakeSystem> awakeSystemMap; //将系统类型映射到 IAwakeSystem 接口的实例
    private Dictionary<Type, IDestroySystem> destroySystemMap;//用于在实体或系统销毁时执行操作的 IDestroySystem

    private Dictionary<Type, IUpdateSystem> updateSystemMap;//每帧都更新的系统
    private Dictionary<IUpdateSystem, List<ECSEntity>> updateSystemRelatedEntityMap;//将IUpdateSystem 实例映射到与它们相关的 ECSEntity 列表的字典

    private Dictionary<Type, ILateUpdateSystem> lateUpdateSystemMap;//lateUpdate更新的系统
    private Dictionary<ILateUpdateSystem, List<ECSEntity>> lateUpdateSystemRelatedEntityMap;

    private Dictionary<Type, IFixedUpdateSystem> fixedUpdateSystemMap;////lateUpdate更新的系统
    private Dictionary<IFixedUpdateSystem, List<ECSEntity>> fixedUpdateSystemRelatedEntityMap;

    private Dictionary<long, ECSEntity> entities = new Dictionary<long, ECSEntity>();//存储实体
    private Dictionary<Type, List<IEntityMessageHandler>> entityMessageHandlerMap;//关于消息
    private Dictionary<Type, IEntityRpcHandler> entityRpcHandlerMap;//远程过程调用（RPC）的处理器


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
    /// 模块初始化时调用
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

        //获取当前调用程序集中的所有类型
        foreach (var type in Assembly.GetCallingAssembly().GetTypes())
        {
            //是否是抽象的，如果是则跳过它
            if (type.IsAbstract)
                continue;

            //是否具有`ECSSystemAttribute`自定义属性
            if (type.GetCustomAttribute<ECSSystemAttribute>(true) != null)
            {
                // AwakeSystem 
                Type awakeSystemType = typeof(IAwakeSystem);
                  //检查类型是否可以实现该接口;
                if (awakeSystemType.IsAssignableFrom(type))
                {
                    //如果已经有一个相同类型的系统存在于对应的映射字典中，则记录一个错误并跳过该类型
                    if (awakeSystemMap.ContainsKey(type))
                    {
                        UnityLog.Error($"Duplicated Awake System:{type.FullName}");
                        continue;
                    }

                    //如果没有重复，使用`Activator.CreateInstance`创建该类型的一个实例
                    IAwakeSystem awakeSystem = Activator.CreateInstance(type) as IAwakeSystem;
                    //将其添加到对应的映射字典中
                    awakeSystemMap.Add(type, awakeSystem);
                }

                // DestroySystem  获取接口类型
                Type destroySystemType = typeof(IDestroySystem);
                //type是否实现了IDestroySystem接口
                if (destroySystemType.IsAssignableFrom(type))
                {
                    //是否包含
                    if (destroySystemMap.ContainsKey(type))
                    {
                        UnityLog.Error($"Duplicated Destroy System:{type.FullName}");
                        continue;
                    }

                    //创建type的一个新实例，并将其转换为IDestroySystem接口类型
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

            //遍历程序集中的所有类型
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
    /// 更新系统
    /// </summary>
    private void DriveUpdateSystem()
    {
        //遍历所有更新系统
        foreach (IUpdateSystem updateSystem in updateSystemMap.Values)
        {
            //得到列表
            List<ECSEntity> updateSystemRelatedEntities = updateSystemRelatedEntityMap[updateSystem];
            if (updateSystemRelatedEntities.Count == 0)
                continue;

            //对象池中获取一个新的列表
            List<ECSEntity> entityList = ListPool<ECSEntity>.Obtain();
            //直接将列表添加
            entityList.AddRangeNonAlloc(updateSystemRelatedEntities);
            foreach (var entity in entityList)
            {
                //当前实体没有被观察 跳过
                if (!updateSystem.ObservingEntity(entity))
                    continue;

                //更新实体
                updateSystem.Update(entity);
            }

            //释放对象回对象池
            ListPool<ECSEntity>.Release(entityList);
        }
    }

    /// <summary>
    /// 更新所有注册了 IUpdateSystem 接口的系统
    /// </summary>
    private void DriveLateUpdateSystem()
    {
        //遍历字典
        foreach (ILateUpdateSystem lateUpdateSystem in lateUpdateSystemMap.Values)
        {
            //获取与更新系统相关的实体
            List<ECSEntity> lateUpdateSystemRelatedEntities = lateUpdateSystemRelatedEntityMap[lateUpdateSystem];
            //检查实体列表是否为空
            if (lateUpdateSystemRelatedEntities.Count == 0)
                continue;

            //对象池中获取新的列表
            List<ECSEntity> entityList = ListPool<ECSEntity>.Obtain();
            //列表添加
            entityList.AddRangeNonAlloc(lateUpdateSystemRelatedEntities);
            foreach (var entity in entityList)
            {
                if (!lateUpdateSystem.ObservingEntity(entity))
                    continue;
                //更新
                lateUpdateSystem.LateUpdate(entity);
            }

            //回收
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
    /// 获取AwakeSystems
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
        //更新系统实体列表
        UpdateSystemEntityList(component.Entity);

        //获取列表
        List<IAwakeSystem> list = ListPool<IAwakeSystem>.Obtain();
        GetAwakeSystems<C>(list);

        //遍历
        bool found = false;
        foreach (var item in list)
        {
            AwakeSystem<C> awakeSystem = item as AwakeSystem<C>;
            if (awakeSystem == null)
                continue;

            //执行逻辑
            awakeSystem.Awake(component);
            found = true;
        }

        //回收释放
        ListPool<IAwakeSystem>.Release(list);
        //没找到 报错
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
    /// 获取销毁列表
    /// </summary>
    /// <typeparam name="C">类型</typeparam>
    /// <param name="list">要添加的列表</param>
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
    /// 获取销毁列表
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
    /// 销毁对应类型的组件
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
    /// 销毁对应类型的组件
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
    /// 销毁对应类型的组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <param name="component"></param>
    /// <param name="p1"></param>
    public void DestroyComponent<C, P1>(C component, P1 p1) where C : ECSComponent
    {
        //更新系统实体列表
        UpdateSystemEntityList(component.Entity);

        //获取销毁系统列表
        List<IDestroySystem> list = ListPool<IDestroySystem>.Obtain();
        //将要销毁的添加到列表
        GetDestroySystems<C>(list);

        //遍历销毁
        foreach (var item in list)
        {
            //转类型
            DestroySystem<C, P1> destroySystem = item as DestroySystem<C, P1>;
            if (destroySystem == null)
                continue;

            //转成功了 调用销毁方法
            destroySystem.Destroy(component, p1);
            //布尔值改变
            component.Disposed = true;
        }

        ListPool<IDestroySystem>.Release(list);
    }

    /// <summary>
    /// 销毁对应类型的组件
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
    /// 更新实体列表
    /// </summary>
    /// <param name="entity"></param>
    private void UpdateSystemEntityList(ECSEntity entity)
    {
        //遍历
        foreach (IUpdateSystem updateSystem in updateSystemMap.Values)
        {
            // update entity list 获取实体列表
            List<ECSEntity> entityList = updateSystemRelatedEntityMap[updateSystem];
            //是否包含该实体
            if (!entityList.Contains(entity))
            {
                //不包含 但是在观察 添加到列表
                if (updateSystem.ObservingEntity(entity))
                {
                    entityList.Add(entity);
                }
            }
            else
            {
                //包含 但是不观察 移除
                if (!updateSystem.ObservingEntity(entity))
                {
                    entityList.Remove(entity);
                }
            }
        }

        //与上面同理
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

        //与上面同理
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
    /// 添加实体
    /// </summary>
    /// <param name="entity"></param>
    public void AddEntity(ECSEntity entity)
    {
        entities.Add(entity.InstanceID, entity);
    }

    /// <summary>
    /// 移除实体
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
    /// 查找实体
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ECSEntity FindEntity(long id)
    {
        return FindEntity<ECSEntity>(id);
    }

    /// <summary>
    /// 查找泛型实体
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
    /// 查找实体的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityID"></param>
    /// <returns></returns>
    public T FindComponentOfEntity<T>(long entityID) where T : ECSComponent
    {
        return FindEntity(entityID)?.GetComponent<T>();
    }

    /// <summary>
    /// 给实体发送消息
    /// </summary>
    /// <typeparam name="M"></typeparam>
    /// <param name="id"></param>
    /// <param name="m"></param>
    /// <returns></returns>
    public async Task SendMessageToEntity<M>(long id, M m)
    {
        //id 为0 不操作
        if (id == 0)
            return;

        //查找实体
        ECSEntity entity = FindEntity(id);
        if (entity == null)
            return;

        //获取消息类型
        Type messageType = m.GetType();
        //获取消息处理列表
        if (!entityMessageHandlerMap.TryGetValue(messageType, out List<IEntityMessageHandler> list))
            return;

        //从对象池中获取
        List<IEntityMessageHandler> entityMessageHandlers = ListPool<IEntityMessageHandler>.Obtain();
        //添加
        entityMessageHandlers.AddRangeNonAlloc(list);
        foreach (IEntityMessageHandler<M> handler in entityMessageHandlers)
        {
            //异步发送消息给实体
            await handler.Post(entity, m);
        }

        //回收释放
        ListPool<IEntityMessageHandler>.Release(entityMessageHandlers);
    }

    /// <summary>
    /// 向指定的实体发送远程过程调用
    /// </summary>
    /// <typeparam name="Request"></typeparam>
    /// <typeparam name="Response"></typeparam>
    /// <param name="entityID">ID</param>
    /// <param name="request">请求</param>
    /// <returns></returns>
    public async Task<Response> SendRpcToEntity<Request, Response>(long entityID, Request request) where Response : IEntityRpcResponse, new()
    {
        //返回一个具有 Error 属性设置为 true 的新 Response 对象
        if (entityID == 0)
            return new Response() { Error = true };

        //没找到实体
        ECSEntity entity = FindEntity(entityID);
        if (entity == null)
            return new Response() { Error = true };

        //获取请求类型
        Type messageType = request.GetType();
        //字典中获取与该请求类型匹配的RPC处理器
        if (!entityRpcHandlerMap.TryGetValue(messageType, out IEntityRpcHandler entityRpcHandler))
            return new Response() { Error = true };

        //将找到的RPC处理器转换为 IEntityRpcHandler<Request, Response> 类型
        IEntityRpcHandler<Request, Response> handler = entityRpcHandler as IEntityRpcHandler<Request, Response>;
        if (handler == null)
            return new Response() { Error = true };

        //使用转换后的RPC处理器（handler）的 Post 方法异步地向实体发送RPC请求，并等待响应
        return await handler.Post(entity, request);
    }
}

