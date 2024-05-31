using Config;
using Nirvana;
using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
//using TGame.Asset;
using UnityEngine;
using UnityEngine.UI;


public partial class UIModule : BaseGameModule
{
  
    public Transform normalUIRoot;  //存储正常UI的根节点
    public Transform modalUIRoot;   //存储模态UI的根节点
    public Transform closeUIRoot;   //存储关闭UI的根节点
    public Image imgMask;           //遮罩层
    public QuantumConsole prefabQuantumConsole; //定义了一个公共的QuantumConsole预制体变量

    private static Dictionary<UIViewID, Type> MEDIATOR_MAPPING; //私有字典  键 UIViewID 值 类型  相应mediator类型
    private static Dictionary<UIViewID, Type> ASSET_MAPPING;    //私有字典  键 UIViewID 值 类型  相应资源类型

    private readonly List<UIMediator> usingMediators = new List<UIMediator>();//只读列表，用于存储当前正在使用的UIMediator对象
    private readonly Dictionary<Type, Queue<UIMediator>> freeMediators = new Dictionary<Type, Queue<UIMediator>>();//管理未使用的UIMediator对象

    //只读GameObjectPool对象，用于管理GameObjectAsset类型的游戏对象池
    private readonly GameObjectPool<GameObjectAsset> uiObjectPool = new GameObjectPool<GameObjectAsset>();
    private QuantumConsole quantumConsole;//存储当前正在使用的QuantumConsole对象

    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();
        //quantumConsole = Instantiate(prefabQuantumConsole);
        //quantumConsole.transform.SetParentAndResetAll(transform);
        //quantumConsole.OnActivate += OnConsoleActive;
        //quantumConsole.OnDeactivate += OnConsoleDeactive;
    }

    protected internal override void OnModuleStop()
    {
        //base.OnModuleStop();
        //quantumConsole.OnActivate -= OnConsoleActive;
        //quantumConsole.OnDeactivate -= OnConsoleDeactive;
    }
    /// <summary>
    /// 缓存UI视图与其对应的Mediator和资源的映射关系
    /// </summary>
    private static void CacheUIMapping()
    {
        //检查映射是否已经存在
        if (MEDIATOR_MAPPING != null)
            return;

        //初始化映射字典
        MEDIATOR_MAPPING = new Dictionary<UIViewID, Type>();
        ASSET_MAPPING = new Dictionary<UIViewID, Type>();

        //获取UIView的基类型
        Type baseViewType = typeof(UIView);
        //遍历程序集中的所有类型
        foreach (var type in baseViewType.Assembly.GetTypes())
        {
            //检查类型是否为抽象类
            if (type.IsAbstract)
                continue;
            //是否继承自UIView
            //IsAssignableFrom 检查当前遍历到的类型 type 是否是 UIView 的子类或实现了 UIView 接口
            if (baseViewType.IsAssignableFrom(type))
            {
                //获取UIViewAttribute
                //GetCustomAttributes 用于检索附加到指定成员的自定义属性的数组
                //type 是一个 Type 对象，它代表了当前正在检查的类或结构的类型
                //false 是一个布尔值，它决定了搜索的范围。当为 false 时，仅检索直接附加到指定成员的自定义属性，不搜索继承的自定义属性。如果为 true，则会检索指定成员及其继承链中的所有自定义属性
                object[] attrs = type.GetCustomAttributes(typeof(UIViewAttribute), false);
                //检查是否有UIViewAttribute
                if (attrs.Length == 0)
                {
                    UnityLog.Error($"{type.FullName} 没有绑定 Mediator，请使用UIMediatorAttribute绑定一个Mediator以正确使用");
                    continue;
                }
                //添加映射关系 将其关联的数据添加进字典
                foreach (UIViewAttribute attr in attrs)
                {
                    MEDIATOR_MAPPING.Add(attr.ID, attr.MediatorType);
                    ASSET_MAPPING.Add(attr.ID, type);
                    break;
                }
            }
        }
    }

    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);
        uiObjectPool.UpdateLoadRequests();
        foreach (var mediator in usingMediators)
        {
            mediator.Update(deltaTime);
        }
        UpdateMask(deltaTime);
    }

    private void OnConsoleActive()
    {
        //GameManager.Input.SetEnable(false);
    }

    private void OnConsoleDeactive()
    {
        //GameManager.Input.SetEnable(true);
    }

    /// <summary>
    /// 得到对应类型的最顶层UIMode
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    private int GetTopMediatorSortingOrder(UIMode mode)
    {
        //用于存储相匹配的最后一个 Mediator
        int lastIndexMediatorOfMode = -1;
        //遍历查找
        for (int i = usingMediators.Count - 1; i >= 0; i--)
        {
            UIMediator mediator = usingMediators[i];
            if (mediator.UIMode != mode)
                continue;

            //找到了 更新索引并退出循环 
            lastIndexMediatorOfMode = i;
            break;
        }

        //检查是否找到匹配的
        if (lastIndexMediatorOfMode == -1)
            return mode == UIMode.Normal ? 0 : 1000;

        //返回它的排序顺序
        return usingMediators[lastIndexMediatorOfMode].SortingOrder;
    }
    /// <summary>
    /// 获取Mediator
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private UIMediator GetMediator(UIViewID id)
    {
        //缓存UI映射关系
        CacheUIMapping();

        //查找Mediator类型
        if (!MEDIATOR_MAPPING.TryGetValue(id, out Type mediatorType))
        {
            UnityLog.Error($"找不到 {id} 对应的Mediator");
            return null;
        }
        //查找与类型关联的队列
        if (!freeMediators.TryGetValue(mediatorType, out Queue<UIMediator> mediatorQ))
        {
            //不存在就创建  并添加
            mediatorQ = new Queue<UIMediator>();
            freeMediators.Add(mediatorType, mediatorQ);
        }

        UIMediator mediator;
        //队列为空
        if (mediatorQ.Count == 0)
        {
            //创建新的实例
            mediator = Activator.CreateInstance(mediatorType) as UIMediator;
        }
        else
        {
            //不为空 取出实例
            mediator = mediatorQ.Dequeue();
        }

        //返回
        return mediator;
    }

    /// <summary>
    /// 回收 Mediator
    /// </summary>
    /// <param name="mediator"></param>
    private void RecycleMediator(UIMediator mediator)
    {
        if (mediator == null)
            return;

        //获取类型
        Type mediatorType = mediator.GetType();
        //字典中获取对应的队列
        if (!freeMediators.TryGetValue(mediatorType, out Queue<UIMediator> mediatorQ))
        {
            //不存在 创建新的
            mediatorQ = new Queue<UIMediator>();
            freeMediators.Add(mediatorType, mediatorQ);
        }
        //存在 直接入队
        mediatorQ.Enqueue(mediator);
    }

    /// <summary>
    /// 获取正在使用的 Mediator
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public UIMediator GetOpeningUIMediator(UIViewID id)
    {
        //获取UI配置
        UIConfig uiConfig = UIConfig.ByID((int)id);
        if (uiConfig.IsNull)
            return null;

        //获取实例
        UIMediator mediator = GetMediator(id);
        if (mediator == null)
            return null;

        //获取类型
        Type requiredMediatorType = mediator.GetType();
        //查找正在使用的
        foreach (var item in usingMediators)
        {
            //找到返回
            if (item.GetType() == requiredMediatorType)
                return item;
        }
        //没找到返回空
        return null;
    }

    /// <summary>
    /// 移到最前面
    /// </summary>
    /// <param name="id"></param>
    public void BringToTop(UIViewID id)
    {
        //获取UIMediator实例
        UIMediator mediator = GetOpeningUIMediator(id);
        if (mediator == null)
            return;

        //检查是否需要调整排序顺序
        int topSortingOrder = GetTopMediatorSortingOrder(mediator.UIMode);
        if (mediator.SortingOrder == topSortingOrder)
            return;

        //调整mediator的排序顺序
        int sortingOrder = topSortingOrder + 10;
        mediator.SortingOrder = sortingOrder;

        //将mediator从usingMediators集合中移除，然后立即重新添加。
        //这个步骤可能是为了确保mediator在集合中的位置是最新的
        usingMediators.Remove(mediator);
        usingMediators.Add(mediator);

        //更新Canvas组件的排序顺序
        Canvas canvas = mediator.ViewObject.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = sortingOrder;
        }
    }

    /// <summary>
    /// 是否在使用
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsUIOpened(UIViewID id)
    {
        return GetOpeningUIMediator(id) != null;
    }

    /// <summary>
    /// 打开特定界面 并只打开一次
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public UIMediator OpenUISingle(UIViewID id, object arg = null)
    {
        UIMediator mediator = GetOpeningUIMediator(id);
        if (mediator != null)
            return mediator;

        return OpenUI(id, arg);
    }

    /// <summary>
    /// 打开UI界面
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public UIMediator OpenUI(UIViewID id, object arg = null)
    {
        //获取UI配置
        UIConfig uiConfig = UIConfig.ByID((int)id);
        if (uiConfig.IsNull)
            return null;

        //获取UIMediator实例
        UIMediator mediator = GetMediator(id);
        if (mediator == null)
            return null;

        //加载UI对象 使用uiObjectPool（可能是一个对象池管理器）来加载或复用UI对象
        GameObject uiObject = (uiObjectPool.LoadGameObject(uiConfig.Asset, (obj) =>
        {
            //获取组件 并初始化
            UIView newView = obj.GetComponent<UIView>();
            mediator.InitMediator(newView);
        })).gameObject;
       

        //处理对象加载完成后的逻辑
        return OnUIObjectLoaded(mediator, uiConfig, uiObject, arg);
    }

    public IEnumerator OpenUISingleAsync(UIViewID id, object arg = null)
    {
        if (!IsUIOpened(id))
        {
            yield return OpenUIAsync(id, arg);
        }
    }

    /// <summary>
    /// 异步加载UI界面
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public IEnumerator OpenUIAsync(UIViewID id, object arg = null)
    {
        //获取UI配置
        UIConfig uiConfig = UIConfig.ByID((int)id);
        if (uiConfig.IsNull)
            yield break;

        //获取UIMediator实例
        UIMediator mediator = GetMediator(id);
        if (mediator == null)
            yield break;

        //异步加载对象
        bool loadFinish = false;
        uiObjectPool.LoadGameObjectAsync(uiConfig.Asset, (asset) =>
        {
            //资源加载完成的回调
            GameObject uiObject = asset.gameObject;
            OnUIObjectLoaded(mediator, uiConfig, uiObject, arg);
            loadFinish = true;
        }, (obj) =>
        {
            //游戏对象加载完成的回调
            //获取组件 并初始化
            UIView newView = obj.GetComponent<UIView>();
            mediator.InitMediator(newView);
        });
        while (!loadFinish)
        {
            yield return null;
        }
        //额外等待了两帧。这通常是为了确保UI对象已经正确加载到场景中，并且所有相关的初始化操作已经完成。
        //这种等待可以是出于确保UI的稳定显示或其他初始化步骤的需要
        yield return null;
        yield return null;
    }

    /// <summary>
    /// 处理对象加载完成后的操作
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="uiConfig"></param>
    /// <param name="uiObject"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    private UIMediator OnUIObjectLoaded(UIMediator mediator, UIConfig uiConfig, GameObject uiObject, object obj)
    {
        //检查加载对象是否为空
        if (uiObject == null)
        {
            UnityLog.Error($"加载UI失败:{uiConfig.Asset}");
            RecycleMediator(mediator);
            return null;
        }

        //是否包含UI对象并显示
        UIView view = uiObject.GetComponent<UIView>();
        if (view == null)
        {
            UnityLog.Error($"UI Prefab不包含UIView脚本:{uiConfig.Asset}");
            RecycleMediator(mediator);
            uiObjectPool.UnloadGameObject(view.gameObject);
            return null;
        }

        //设置mediator的UI模式和排序顺序
        mediator.UIMode = uiConfig.Mode;
        int sortingOrder = GetTopMediatorSortingOrder(uiConfig.Mode) + 10;

        //添加mediator到usingMediators集合中
        usingMediators.Add(mediator);

        //设置Canvas的渲染模式和父对象
        Canvas canvas = uiObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //canvas.worldCamera = GameManager.Camera.uiCamera;

        //根据UI模式设置UI对象的父对象和排序层
        if (uiConfig.Mode == UIMode.Normal)
        {
            uiObject.transform.SetParentAndResetAll(normalUIRoot);
            canvas.sortingLayerName = "NormalUI";
        }
        else
        {
            uiObject.transform.SetParentAndResetAll(modalUIRoot);
            canvas.sortingLayerName = "ModalUI";
        }

        //设置mediator和Canvas的排序顺序
        mediator.SortingOrder = sortingOrder;
        canvas.sortingOrder = sortingOrder;
        
        //激活UI对象并显示
        uiObject.SetActive(true);
        mediator.Show(uiObject, obj);
        return mediator;
    }

    public void CloseUI(UIMediator mediator)
    {
        if (mediator != null)
        {
            // 回收View
            uiObjectPool.UnloadGameObject(mediator.ViewObject);
            mediator.ViewObject.transform.SetParentAndResetAll(closeUIRoot);

            // 回收Mediator
            mediator.Hide();
            RecycleMediator(mediator);

            usingMediators.Remove(mediator);
        }
    }

    /// <summary>
    /// 关闭所有UI
    /// </summary>
    public void CloseAllUI()
    {
        for (int i = usingMediators.Count - 1; i >= 0; i--)
        {
            CloseUI(usingMediators[i]);
        }
    }

    /// <summary>
    /// 关闭IUI
    /// </summary>
    /// <param name="id"></param>
    public void CloseUI(UIViewID id)
    {
        UIMediator mediator = GetOpeningUIMediator(id);
        if (mediator == null)
            return;

        CloseUI(mediator);
    }

    /// <summary>
    /// 设置所有普通UI的可见性
    /// </summary>
    /// <param name="visible"></param>
    public void SetAllNormalUIVisibility(bool visible)
    {
        normalUIRoot.gameObject.SetActive(visible);
    }

    /// <summary>
    /// 设置所有ModalUI的可见性
    /// </summary>
    /// <param name="visible"></param>
    public void SetAllModalUIVisibility(bool visible)
    {
        modalUIRoot.gameObject.SetActive(visible);
    }

    /// <summary>
    /// 显示遮罩
    /// </summary>
    /// <param name="duration"></param>
    public void ShowMask(float duration = 0.5f)
    {
        destMaskAlpha = 1;
        maskDuration = duration;
    }

    /// <summary>
    /// 隐藏遮罩
    /// </summary>
    /// <param name="duration"></param>
    public void HideMask(float? duration = null)
    {
        destMaskAlpha = 0;
        if (duration.HasValue)
        {
            maskDuration = duration.Value;
        }
    }

    private float destMaskAlpha = 0;//目标透明度
    private float maskDuration = 0;//动画时长
    /// <summary>
    /// 更新遮罩透明度
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateMask(float deltaTime)
    {
        //获取imgMask组件当前的颜色值，并将其存储在变量c中
        Color c = imgMask.color;
        //如果maskDuration大于0，表示有动画时长，那么使用Mathf.MoveTowards函数来平滑地改变透明度。这个函数会将c.a（当前透明度）逐渐移动到destMaskAlpha（目标透明度），移动的速度由1f / maskDuration * deltaTime决定
        //如果maskDuration不大于0（即没有动画时长），则直接将c.a设置为destMaskAlpha
        c.a = maskDuration > 0 ? Mathf.MoveTowards(c.a, destMaskAlpha, 1f / maskDuration * deltaTime) : destMaskAlpha;
        //确保透明度值c.a始终在0到1之间，即不会超出透明和完全不透明的范围。
        c.a = Mathf.Clamp01(c.a);
        //将更新后的颜色值（包括新的透明度）设置回imgMask组件
        imgMask.color = c;
        //mgMask.enabled = imgMask.color.a > 0;：根据新的透明度值来决定是否启用imgMask组件。
        //如果透明度大于0（即不是完全透明），则启用该组件；否则禁用
        imgMask.enabled = imgMask.color.a > 0;
    }

    public void ShowConsole()
    {
        quantumConsole.Activate();
    }
}


[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class UIViewAttribute : Attribute
{
    public UIViewID ID { get; }
    public Type MediatorType { get; }

    public UIViewAttribute(Type mediatorType, UIViewID id)
    {
        ID = id;
        MediatorType = mediatorType;
    }
}
