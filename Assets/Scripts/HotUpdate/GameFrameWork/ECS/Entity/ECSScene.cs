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
        //检查是否已经释放
        if (Disposed)
            return;

        //获取实例
        List<long> entityIDList = ListPool<long>.Obtain();

        foreach (var entityID in entities.Keys)
        {
            //将这些ID添加到先前从对象池中获取的列表中
            entityIDList.Add(entityID);
        }

        //释放实体资源
        foreach (var entityID in entityIDList)
        {
            ECSEntity entity = entities[entityID];
            entity.Dispose();
        }

        //释放列表
        ListPool<long>.Release(entityIDList);

        base.Dispose();
    }

    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity"></param>
    public void AddEntity(ECSEntity entity)
    {
        if (entity == null)
            return;

        //获取 entity 当前所属的场景（Scene）的引用
        ECSScene oldScene = entity.Scene;
        if (oldScene != null)
        {
            //将实体从旧场景中移除
            oldScene.RemoveEntity(entity.InstanceID);
        }

        //将实体添加到当前场景
        entities.Add(entity.InstanceID, entity);
        //设置ID
        entity.SceneID = InstanceID;
        UnityLog.Info($"Scene Add Entity, Current Count:{entities.Count}");
    }

    /// <summary>
    /// 移除实体
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
    /// 查找实体
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
    /// 查找实体组件
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
    /// 获取所有实体
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
