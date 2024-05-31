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
  
    public Transform normalUIRoot;  //�洢����UI�ĸ��ڵ�
    public Transform modalUIRoot;   //�洢ģ̬UI�ĸ��ڵ�
    public Transform closeUIRoot;   //�洢�ر�UI�ĸ��ڵ�
    public Image imgMask;           //���ֲ�
    public QuantumConsole prefabQuantumConsole; //������һ��������QuantumConsoleԤ�������

    private static Dictionary<UIViewID, Type> MEDIATOR_MAPPING; //˽���ֵ�  �� UIViewID ֵ ����  ��Ӧmediator����
    private static Dictionary<UIViewID, Type> ASSET_MAPPING;    //˽���ֵ�  �� UIViewID ֵ ����  ��Ӧ��Դ����

    private readonly List<UIMediator> usingMediators = new List<UIMediator>();//ֻ���б����ڴ洢��ǰ����ʹ�õ�UIMediator����
    private readonly Dictionary<Type, Queue<UIMediator>> freeMediators = new Dictionary<Type, Queue<UIMediator>>();//����δʹ�õ�UIMediator����

    //ֻ��GameObjectPool�������ڹ���GameObjectAsset���͵���Ϸ�����
    private readonly GameObjectPool<GameObjectAsset> uiObjectPool = new GameObjectPool<GameObjectAsset>();
    private QuantumConsole quantumConsole;//�洢��ǰ����ʹ�õ�QuantumConsole����

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
    /// ����UI��ͼ�����Ӧ��Mediator����Դ��ӳ���ϵ
    /// </summary>
    private static void CacheUIMapping()
    {
        //���ӳ���Ƿ��Ѿ�����
        if (MEDIATOR_MAPPING != null)
            return;

        //��ʼ��ӳ���ֵ�
        MEDIATOR_MAPPING = new Dictionary<UIViewID, Type>();
        ASSET_MAPPING = new Dictionary<UIViewID, Type>();

        //��ȡUIView�Ļ�����
        Type baseViewType = typeof(UIView);
        //���������е���������
        foreach (var type in baseViewType.Assembly.GetTypes())
        {
            //��������Ƿ�Ϊ������
            if (type.IsAbstract)
                continue;
            //�Ƿ�̳���UIView
            //IsAssignableFrom ��鵱ǰ������������ type �Ƿ��� UIView �������ʵ���� UIView �ӿ�
            if (baseViewType.IsAssignableFrom(type))
            {
                //��ȡUIViewAttribute
                //GetCustomAttributes ���ڼ������ӵ�ָ����Ա���Զ������Ե�����
                //type ��һ�� Type �����������˵�ǰ���ڼ������ṹ������
                //false ��һ������ֵ���������������ķ�Χ����Ϊ false ʱ��������ֱ�Ӹ��ӵ�ָ����Ա���Զ������ԣ��������̳е��Զ������ԡ����Ϊ true��������ָ����Ա����̳����е������Զ�������
                object[] attrs = type.GetCustomAttributes(typeof(UIViewAttribute), false);
                //����Ƿ���UIViewAttribute
                if (attrs.Length == 0)
                {
                    UnityLog.Error($"{type.FullName} û�а� Mediator����ʹ��UIMediatorAttribute��һ��Mediator����ȷʹ��");
                    continue;
                }
                //���ӳ���ϵ ���������������ӽ��ֵ�
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
    /// �õ���Ӧ���͵����UIMode
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    private int GetTopMediatorSortingOrder(UIMode mode)
    {
        //���ڴ洢��ƥ������һ�� Mediator
        int lastIndexMediatorOfMode = -1;
        //��������
        for (int i = usingMediators.Count - 1; i >= 0; i--)
        {
            UIMediator mediator = usingMediators[i];
            if (mediator.UIMode != mode)
                continue;

            //�ҵ��� �����������˳�ѭ�� 
            lastIndexMediatorOfMode = i;
            break;
        }

        //����Ƿ��ҵ�ƥ���
        if (lastIndexMediatorOfMode == -1)
            return mode == UIMode.Normal ? 0 : 1000;

        //������������˳��
        return usingMediators[lastIndexMediatorOfMode].SortingOrder;
    }
    /// <summary>
    /// ��ȡMediator
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private UIMediator GetMediator(UIViewID id)
    {
        //����UIӳ���ϵ
        CacheUIMapping();

        //����Mediator����
        if (!MEDIATOR_MAPPING.TryGetValue(id, out Type mediatorType))
        {
            UnityLog.Error($"�Ҳ��� {id} ��Ӧ��Mediator");
            return null;
        }
        //���������͹����Ķ���
        if (!freeMediators.TryGetValue(mediatorType, out Queue<UIMediator> mediatorQ))
        {
            //�����ھʹ���  �����
            mediatorQ = new Queue<UIMediator>();
            freeMediators.Add(mediatorType, mediatorQ);
        }

        UIMediator mediator;
        //����Ϊ��
        if (mediatorQ.Count == 0)
        {
            //�����µ�ʵ��
            mediator = Activator.CreateInstance(mediatorType) as UIMediator;
        }
        else
        {
            //��Ϊ�� ȡ��ʵ��
            mediator = mediatorQ.Dequeue();
        }

        //����
        return mediator;
    }

    /// <summary>
    /// ���� Mediator
    /// </summary>
    /// <param name="mediator"></param>
    private void RecycleMediator(UIMediator mediator)
    {
        if (mediator == null)
            return;

        //��ȡ����
        Type mediatorType = mediator.GetType();
        //�ֵ��л�ȡ��Ӧ�Ķ���
        if (!freeMediators.TryGetValue(mediatorType, out Queue<UIMediator> mediatorQ))
        {
            //������ �����µ�
            mediatorQ = new Queue<UIMediator>();
            freeMediators.Add(mediatorType, mediatorQ);
        }
        //���� ֱ�����
        mediatorQ.Enqueue(mediator);
    }

    /// <summary>
    /// ��ȡ����ʹ�õ� Mediator
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public UIMediator GetOpeningUIMediator(UIViewID id)
    {
        //��ȡUI����
        UIConfig uiConfig = UIConfig.ByID((int)id);
        if (uiConfig.IsNull)
            return null;

        //��ȡʵ��
        UIMediator mediator = GetMediator(id);
        if (mediator == null)
            return null;

        //��ȡ����
        Type requiredMediatorType = mediator.GetType();
        //��������ʹ�õ�
        foreach (var item in usingMediators)
        {
            //�ҵ�����
            if (item.GetType() == requiredMediatorType)
                return item;
        }
        //û�ҵ����ؿ�
        return null;
    }

    /// <summary>
    /// �Ƶ���ǰ��
    /// </summary>
    /// <param name="id"></param>
    public void BringToTop(UIViewID id)
    {
        //��ȡUIMediatorʵ��
        UIMediator mediator = GetOpeningUIMediator(id);
        if (mediator == null)
            return;

        //����Ƿ���Ҫ��������˳��
        int topSortingOrder = GetTopMediatorSortingOrder(mediator.UIMode);
        if (mediator.SortingOrder == topSortingOrder)
            return;

        //����mediator������˳��
        int sortingOrder = topSortingOrder + 10;
        mediator.SortingOrder = sortingOrder;

        //��mediator��usingMediators�������Ƴ���Ȼ������������ӡ�
        //������������Ϊ��ȷ��mediator�ڼ����е�λ�������µ�
        usingMediators.Remove(mediator);
        usingMediators.Add(mediator);

        //����Canvas���������˳��
        Canvas canvas = mediator.ViewObject.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = sortingOrder;
        }
    }

    /// <summary>
    /// �Ƿ���ʹ��
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsUIOpened(UIViewID id)
    {
        return GetOpeningUIMediator(id) != null;
    }

    /// <summary>
    /// ���ض����� ��ֻ��һ��
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
    /// ��UI����
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public UIMediator OpenUI(UIViewID id, object arg = null)
    {
        //��ȡUI����
        UIConfig uiConfig = UIConfig.ByID((int)id);
        if (uiConfig.IsNull)
            return null;

        //��ȡUIMediatorʵ��
        UIMediator mediator = GetMediator(id);
        if (mediator == null)
            return null;

        //����UI���� ʹ��uiObjectPool��������һ������ع������������ػ���UI����
        GameObject uiObject = (uiObjectPool.LoadGameObject(uiConfig.Asset, (obj) =>
        {
            //��ȡ��� ����ʼ��
            UIView newView = obj.GetComponent<UIView>();
            mediator.InitMediator(newView);
        })).gameObject;
       

        //������������ɺ���߼�
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
    /// �첽����UI����
    /// </summary>
    /// <param name="id"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public IEnumerator OpenUIAsync(UIViewID id, object arg = null)
    {
        //��ȡUI����
        UIConfig uiConfig = UIConfig.ByID((int)id);
        if (uiConfig.IsNull)
            yield break;

        //��ȡUIMediatorʵ��
        UIMediator mediator = GetMediator(id);
        if (mediator == null)
            yield break;

        //�첽���ض���
        bool loadFinish = false;
        uiObjectPool.LoadGameObjectAsync(uiConfig.Asset, (asset) =>
        {
            //��Դ������ɵĻص�
            GameObject uiObject = asset.gameObject;
            OnUIObjectLoaded(mediator, uiConfig, uiObject, arg);
            loadFinish = true;
        }, (obj) =>
        {
            //��Ϸ���������ɵĻص�
            //��ȡ��� ����ʼ��
            UIView newView = obj.GetComponent<UIView>();
            mediator.InitMediator(newView);
        });
        while (!loadFinish)
        {
            yield return null;
        }
        //����ȴ�����֡����ͨ����Ϊ��ȷ��UI�����Ѿ���ȷ���ص������У�����������صĳ�ʼ�������Ѿ���ɡ�
        //���ֵȴ������ǳ���ȷ��UI���ȶ���ʾ��������ʼ���������Ҫ
        yield return null;
        yield return null;
    }

    /// <summary>
    /// ������������ɺ�Ĳ���
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="uiConfig"></param>
    /// <param name="uiObject"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    private UIMediator OnUIObjectLoaded(UIMediator mediator, UIConfig uiConfig, GameObject uiObject, object obj)
    {
        //�����ض����Ƿ�Ϊ��
        if (uiObject == null)
        {
            UnityLog.Error($"����UIʧ��:{uiConfig.Asset}");
            RecycleMediator(mediator);
            return null;
        }

        //�Ƿ����UI������ʾ
        UIView view = uiObject.GetComponent<UIView>();
        if (view == null)
        {
            UnityLog.Error($"UI Prefab������UIView�ű�:{uiConfig.Asset}");
            RecycleMediator(mediator);
            uiObjectPool.UnloadGameObject(view.gameObject);
            return null;
        }

        //����mediator��UIģʽ������˳��
        mediator.UIMode = uiConfig.Mode;
        int sortingOrder = GetTopMediatorSortingOrder(uiConfig.Mode) + 10;

        //���mediator��usingMediators������
        usingMediators.Add(mediator);

        //����Canvas����Ⱦģʽ�͸�����
        Canvas canvas = uiObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        //canvas.worldCamera = GameManager.Camera.uiCamera;

        //����UIģʽ����UI����ĸ�����������
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

        //����mediator��Canvas������˳��
        mediator.SortingOrder = sortingOrder;
        canvas.sortingOrder = sortingOrder;
        
        //����UI������ʾ
        uiObject.SetActive(true);
        mediator.Show(uiObject, obj);
        return mediator;
    }

    public void CloseUI(UIMediator mediator)
    {
        if (mediator != null)
        {
            // ����View
            uiObjectPool.UnloadGameObject(mediator.ViewObject);
            mediator.ViewObject.transform.SetParentAndResetAll(closeUIRoot);

            // ����Mediator
            mediator.Hide();
            RecycleMediator(mediator);

            usingMediators.Remove(mediator);
        }
    }

    /// <summary>
    /// �ر�����UI
    /// </summary>
    public void CloseAllUI()
    {
        for (int i = usingMediators.Count - 1; i >= 0; i--)
        {
            CloseUI(usingMediators[i]);
        }
    }

    /// <summary>
    /// �ر�IUI
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
    /// ����������ͨUI�Ŀɼ���
    /// </summary>
    /// <param name="visible"></param>
    public void SetAllNormalUIVisibility(bool visible)
    {
        normalUIRoot.gameObject.SetActive(visible);
    }

    /// <summary>
    /// ��������ModalUI�Ŀɼ���
    /// </summary>
    /// <param name="visible"></param>
    public void SetAllModalUIVisibility(bool visible)
    {
        modalUIRoot.gameObject.SetActive(visible);
    }

    /// <summary>
    /// ��ʾ����
    /// </summary>
    /// <param name="duration"></param>
    public void ShowMask(float duration = 0.5f)
    {
        destMaskAlpha = 1;
        maskDuration = duration;
    }

    /// <summary>
    /// ��������
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

    private float destMaskAlpha = 0;//Ŀ��͸����
    private float maskDuration = 0;//����ʱ��
    /// <summary>
    /// ��������͸����
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateMask(float deltaTime)
    {
        //��ȡimgMask�����ǰ����ɫֵ��������洢�ڱ���c��
        Color c = imgMask.color;
        //���maskDuration����0����ʾ�ж���ʱ������ôʹ��Mathf.MoveTowards������ƽ���ظı�͸���ȡ���������Ὣc.a����ǰ͸���ȣ����ƶ���destMaskAlpha��Ŀ��͸���ȣ����ƶ����ٶ���1f / maskDuration * deltaTime����
        //���maskDuration������0����û�ж���ʱ��������ֱ�ӽ�c.a����ΪdestMaskAlpha
        c.a = maskDuration > 0 ? Mathf.MoveTowards(c.a, destMaskAlpha, 1f / maskDuration * deltaTime) : destMaskAlpha;
        //ȷ��͸����ֵc.aʼ����0��1֮�䣬�����ᳬ��͸������ȫ��͸���ķ�Χ��
        c.a = Mathf.Clamp01(c.a);
        //�����º����ɫֵ�������µ�͸���ȣ����û�imgMask���
        imgMask.color = c;
        //mgMask.enabled = imgMask.color.a > 0;�������µ�͸����ֵ�������Ƿ�����imgMask�����
        //���͸���ȴ���0����������ȫ͸�����������ø�������������
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
