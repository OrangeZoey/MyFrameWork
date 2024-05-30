using System;
using System.Collections.Generic;


public class ECSEntity : IDisposable
{
    public long InstanceID { get; private set; } //ID
    public long ParentID { get; private set; }  //父类ID
    public bool Disposed { get; private set; } //是否释放

    public ECSEntity Parent //定义一个 ECSEntity 类型的 Parent 属性
    {
        get
        {
            if (ParentID == 0)
                return default;  //没有父实体 返回默认值

            return TGameFramework.Instance.GetModule<ECSModule>().FindEntity(ParentID); //查找父实体
        }
    }

    public long SceneID { get; set; } //场景ID
    public ECSScene Scene //定义一个 ECSScene 类型的 Scene 属性
    {
        get
        {
            if (SceneID == 0)
                return default;

            return TGameFramework.Instance.GetModule<ECSModule>().FindEntity(SceneID) as ECSScene;
        }
    }

    private List<ECSEntity> children = new List<ECSEntity>(); //孩子
    private Dictionary<Type, ECSComponent> componentMap = new Dictionary<Type, ECSComponent>(); //组件

    public ECSEntity()
    {

        //得到ID
        InstanceID = IDGenerator.NewInstanceID();
        //获取模块
        TGameFramework.Instance.GetModule<ECSModule>().AddEntity(this);
    }

    public virtual void Dispose()
    {
        if (Disposed)
            return;

        Disposed = true;
        // 销毁Child
        for (int i = children.Count - 1; i >= 0; i--)
        {
            ECSEntity child = children[i];
            children.RemoveAt(i);
            child?.Dispose();
        }

        // 销毁Component
        List<ECSComponent> componentList = ListPool<ECSComponent>.Obtain();
        foreach (var component in componentMap.Values)
        {
            componentList.Add(component);
        }

        foreach (var component in componentList)
        {
            componentMap.Remove(component.GetType());
            TGameFramework.Instance.GetModule<ECSModule>().DestroyComponent(component);
        }
        ListPool<ECSComponent>.Release(componentList);

        // 从父节点移除
        Parent?.RemoveChild(this);
        // 从世界中移除
        TGameFramework.Instance.GetModule<ECSModule>().RemoveEntity(this);
    }

    /// <summary>
    /// 组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public bool HasComponent<C>() where C : ECSComponent
    {
        //返回对应组件
        return componentMap.ContainsKey(typeof(C));
    }

    /// <summary>
    /// 获取组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public C GetComponent<C>() where C : ECSComponent
    {
        componentMap.TryGetValue(typeof(C), out var component);
        return component as C;
    }

    /// <summary>
    /// 添加新组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <returns></returns>
    public C AddNewComponent<C>() where C : ECSComponent, new()
    {
        //获取旧组件
        if (HasComponent<C>())
        {
            //移除组件
            RemoveComponent<C>();
        }

        //创建新组件
        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component);
        return component;
    }

    public C AddNewComponent<C, P1>(P1 p1) where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            RemoveComponent<C>();
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component, p1);
        return component;
    }

    public C AddNewComponent<C, P1, P2>(P1 p1, P2 p2) where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            RemoveComponent<C>();
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component, p1, p2);
        return component;
    }

    public C AddComponent<C>() where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            UnityLog.Error($"Duplicated Component:{typeof(C).FullName}");
            return default;
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component);
        return component;
    }

    public C AddComponent<C, P1>(P1 p1) where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            UnityLog.Error($"Duplicated Component:{typeof(C).FullName}");
            return default;
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component, p1);
        return component;
    }

    public C AddComponent<C, P1, P2>(P1 p1, P2 p2) where C : ECSComponent, new()
    {
        if (HasComponent<C>())
        {
            UnityLog.Error($"Duplicated Component:{typeof(C).FullName}");
            return default;
        }

        C component = new C();
        component.EntityID = InstanceID;
        componentMap.Add(typeof(C), component);
        TGameFramework.Instance.GetModule<ECSModule>().AwakeComponent(component, p1, p2);
        return component;
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    public void RemoveComponent<C>() where C : ECSComponent, new()
    {
        //获取类型
        Type componentType = typeof(C);
        if (!componentMap.TryGetValue(componentType, out var component))
            return;

        //字典中移除
        componentMap.Remove(componentType);
        //获取模块
        TGameFramework.Instance.GetModule<ECSModule>().DestroyComponent((C)component);
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <param name="p1"></param>
    public void RemoveComponent<C, P1>(P1 p1) where C : ECSComponent, new()
    {
        Type componentType = typeof(C);
        if (!componentMap.TryGetValue(componentType, out var component))
            return;

        componentMap.Remove(componentType);
        TGameFramework.Instance.GetModule<ECSModule>().DestroyComponent((C)component, p1);
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="P1"></typeparam>
    /// <typeparam name="P2"></typeparam>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public void RemoveComponent<C, P1, P2>(P1 p1, P2 p2) where C : ECSComponent, new()
    {
        Type componentType = typeof(C);
        if (!componentMap.TryGetValue(componentType, out var component))
            return;

        componentMap.Remove(componentType);
        TGameFramework.Instance.GetModule<ECSModule>().DestroyComponent((C)component, p1, p2);
    }

    /// <summary>
    /// 添加孩子
    /// </summary>
    /// <param name="child"></param>
    public void AddChild(ECSEntity child)
    {
        if (child == null)
            return;

        //孩子释放了
        if (child.Disposed)
            return;

        //旧的父实体
        ECSEntity oldParent = child.Parent;
        if (oldParent != null)
        {
            //旧实体移除孩子
            oldParent.RemoveChild(child);
        }

        //本实体孩子列表添加
        children.Add(child);
        //赋值ID
        child.ParentID = InstanceID;
    }

    /// <summary>
    /// 移除孩子
    /// </summary>
    /// <param name="child"></param>
    public void RemoveChild(ECSEntity child)
    {
        if (child == null)
            return;

        children.Remove(child);
        child.ParentID = 0;
    }

    /// <summary>
    /// 查找孩子
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public T FindChild<T>(long id) where T : ECSEntity
    {
        foreach (var child in children)
        {
            if (child.InstanceID == id)
                return child as T;
        }

        return default;
    }

    /// <summary>
    /// 查找孩子
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate">委托</param>
    /// <returns></returns>
    public T FindChild<T>(Predicate<T> predicate) where T : ECSEntity
    {
        foreach (var child in children)
        {
            T c = child as T;
            if (c == null)
                continue;

            //调用委托
            if (predicate.Invoke(c))
            {
                return c;
            }
        }

        return default;
    }

    /// <summary>
    /// 从孩子列表查找对应类型的 添加到列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public void FindChildren<T>(List<T> list) where T : ECSEntity
    {
        foreach (var child in children)
        {
            if (child is T)
            {
                list.Add(child as T);
            }
        }
    }
}

