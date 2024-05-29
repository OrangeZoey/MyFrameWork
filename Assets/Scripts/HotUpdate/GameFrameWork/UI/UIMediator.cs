using Config;
using Nirvana;
using System.Xml;
using UnityEngine;


public abstract class UIMediator<T> : UIMediator where T : UIView
{
    //view
    protected T view;

    /// <summary>
    /// ��ʾ
    /// </summary>
    /// <param name="arg"></param>
    protected override void OnShow(object arg)
    {
        base.OnShow(arg);
        view = ViewObject.GetComponent<T>();

    }
    /// <summary>
    /// ����
    /// </summary>
    protected override void OnHide()
    {
        view = default;
        base.OnHide();
    }

    /// <summary>
    /// �ر�
    /// </summary>
    protected void Close()
    {
        TGameFramework.Instance.GetModule<UIModule>().CloseUI(this);
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="view"></param>
    public override void InitMediator(UIView view)
    {
        base.InitMediator(view);

        OnInit(view as T);
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="view"></param>
    protected virtual void OnInit(T view) { }
}

public abstract class UIMediator
{

    public event System.Action OnMediatorHide;//�¼�
    public GameObject ViewObject { get; set; } //view��ʵ�����
    public UIEventTable eventTable { get; set; }// UIEventTable����
    public UINameTable nameTable { get; set; } //UINameTable ����
    public int SortingOrder { get; set; } //����ȷ����Ⱦ˳��  ֵԽ����Ⱦ˳��Խ��
    public UIMode UIMode { get; set; }  //UIMode ����

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="view"></param>
    public virtual void InitMediator(UIView view) { }

    /// <summary>
    /// ��ʾ
    /// </summary>
    /// <param name="viewObject"></param>
    /// <param name="arg"></param>
    public void Show(GameObject viewObject, object arg)
    {
        //���Ը�ֵ
        ViewObject = viewObject;
        eventTable = ViewObject.GetComponent<UIEventTable>();
        nameTable = viewObject.GetComponent<UINameTable>();
        OnShow(arg);
    }
    protected virtual void OnShow(object arg) { }

    /// <summary>
    /// ����
    /// </summary>
    public void Hide()
    {
        OnHide();
        //�����¼�
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

