using System.Collections.Generic;


public class ECSScene : ECSEntity
{
    private Dictionary<long, ECSEntity> entities;

    public ECSScene()
    {
        entities = new Dictionary<long, ECSEntity>();
    }

    public override void Dispose()
    {
        //����Ƿ��Ѿ��ͷ�
        if (Disposed)
            return;

        //��ȡʵ��
        List<long> entityIDList = ListPool<long>.Obtain();

        foreach (var entityID in entities.Keys)
        {
            //����ЩID��ӵ���ǰ�Ӷ�����л�ȡ���б���
            entityIDList.Add(entityID);
        }

        //�ͷ�ʵ����Դ
        foreach (var entityID in entityIDList)
        {
            ECSEntity entity = entities[entityID];
            entity.Dispose();
        }

        //�ͷ��б�
        ListPool<long>.Release(entityIDList);

        base.Dispose();
    }

    /// <summary>
    /// ���ʵ��
    /// </summary>
    /// <param name="entity"></param>
    public void AddEntity(ECSEntity entity)
    {
        if (entity == null)
            return;

        //��ȡ entity ��ǰ�����ĳ�����Scene��������
        ECSScene oldScene = entity.Scene;
        if (oldScene != null)
        {
            //��ʵ��Ӿɳ������Ƴ�
            oldScene.RemoveEntity(entity.InstanceID);
        }

        //��ʵ����ӵ���ǰ����
        entities.Add(entity.InstanceID, entity);
        //����ID
        entity.SceneID = InstanceID;
        UnityLog.Info($"Scene Add Entity, Current Count:{entities.Count}");
    }

    /// <summary>
    /// �Ƴ�ʵ��
    /// </summary>
    /// <param name="entityID"></param>
    public void RemoveEntity(long entityID)
    {
        if (entities.Remove(entityID))
        {
            UnityLog.Info($"Scene Remove Entity, Current Count:{entities.Count}");
        }
    }

    /// <summary>
    /// ����ʵ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public void FindEntities<T>(List<long> list) where T : ECSEntity
    {
        foreach (var item in entities)
        {
            if (item.Value is T)
            {
                list.Add(item.Key);
            }
        }
    }

    /// <summary>
    /// ����ʵ�����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public void FindEntitiesWithComponent<T>(List<long> list) where T : ECSComponent
    {
        foreach (var item in entities)
        {
            if (item.Value.HasComponent<T>())
            {
                list.Add(item.Key);
            }
        }
    }

    /// <summary>
    /// ��ȡ����ʵ��
    /// </summary>
    /// <param name="list"></param>
    public void GetAllEntities(List<long> list)
    {
        foreach (var item in entities)
        {
            list.Add(item.Key);
        }
    }
}
