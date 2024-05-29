using Config;
using Nirvana;
using System.Xml;
using UnityEngine;


public abstract class UIMediator<T> : UIMediator where T : UIView
{
    //view
    protected T view;

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="arg"></param>
    protected override void OnShow(object arg)
    {
        base.OnShow(arg);
        view = ViewObject.GetComponent<T>();

    }
    /// <summary>
    /// 隐藏
    /// </summary>
    protected override void OnHide()
    {
        view = default;
        base.OnHide();
    }

    /// <summary>
    /// 关闭
    /// </summary>
    protected void Close()
    {
        TGameFramework.Instance.GetModule<UIModule>().CloseUI(this);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="view"></param>
    public override void InitMediator(UIView view)
    {
        base.InitMediator(view);

        OnInit(view as T);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="view"></param>
    protected virtual void OnInit(T view) { }
}

public abstract class UIMediator
{

    public event System.Action OnMediatorHide;//事件
    public GameObject ViewObject { get; set; } //view的实体对象
    public UIEventTable eventTable { get; set; }// UIEventTable属性
    public UINameTable nameTable { get; set; } //UINameTable 属性
    public int SortingOrder { get; set; } //用于确定渲染顺序  值越大渲染顺序越高
    public UIMode UIMode { get; set; }  //UIMode 属性

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="view"></param>
    public virtual void InitMediator(UIView view) { }

    /// <summary>
    /// 显示
    /// </summary>
    /// <param name="viewObject"></param>
    /// <param name="arg"></param>
    public void Show(GameObject viewObject, object arg)
    {
        //属性赋值
        ViewObject = viewObject;
        eventTable = ViewObject.GetComponent<UIEventTable>();
        nameTable = viewObject.GetComponent<UINameTable>();
        OnShow(arg);
    }
    protected virtual void OnShow(object arg) { }

    /// <summary>
    /// 隐藏
    /// </summary>
    public void Hide()
    {
        OnHide();
        //触发事件
        OnMediatorHide?.Invoke();
        OnMediatorHide = null;
        ViewObject = default;
    }

    protected virtual void OnHide() { }

    public void Update(float deltaTime)
    {
        OnUpdate(deltaTime);

    }

    protected virtual void OnUpdate(float deltaTime) { }
}

