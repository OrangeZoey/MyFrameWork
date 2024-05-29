using Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// ��Դ���
    /// </summary>
    [Module(1)]
    public static AssetModule Asset { get => TGameFramework.Instance.GetModule<AssetModule>(); }

    /// <summary>
    /// �������
    /// </summary>

    [Module(2)]
    public static ProcedureModule Procedure { get => TGameFramework.Instance.GetModule<ProcedureModule>(); }


    [Module(3)]
    public static UIModule UI { get => TGameFramework.Instance.GetModule<UIModule>(); }

    [Module(6)]
    public static MessageModule Message { get => TGameFramework.Instance.GetModule<MessageModule>(); }


    [Module(7)]
    public static ECSModule ECS { get => TGameFramework.Instance.GetModule<ECSModule>(); }


    [Module(97)]
    public static NetModule Net { get => TGameFramework.Instance.GetModule<NetModule>(); }

    private bool activing;

    private void Awake() 
    {
        if (TGameFramework.Instance!=null)
        {
            Destroy(gameObject);
            return;
        }

        activing = true;
        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        //UnityLog.StartupEditor();
#else
            //UnityLog.Startup();
#endif

        Application.logMessageReceived += OnReceiveLog;
        TGameFramework.Initialize();
        StartupModules();
        TGameFramework.Instance.InitModules();

        //��������
        ConfigManager.LoadAllConfigsByAddressable("Assets/BundleAssets/Config");
        GlobalConfig.InitGlobalConfig();
        BuffConfig.ParseConfig();
        SkillConfig.ParseConfig();
        BulletConfig.ParseConfig();
        SpellFieldConfig.ParseConfig();
        I18NConfig.ParseConfig();
    }

    private void Start()
    {
        TGameFramework.Instance.StartModules();
        Procedure.StartProcedure().Coroutine();
    }

    private void Update()
    {
        TGameFramework.Instance.Update();
    }

    private void LateUpdate()
    {
        TGameFramework.Instance.LateUpdate();
    }

    private void FixedUpdate()
    {
        //  TGameFramework.Instance.FixedUpdate();
    }

    private void OnDestroy()
    {
        if (activing)
        {
            Application.logMessageReceived -= OnReceiveLog;
            TGameFramework.Instance.Destroy();
        }
    }

    private void OnApplicationQuit()
    {
        //UnityLog.Teardown();
    }


    /// <summary>
    /// ��ʼ��ģ��
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void StartupModules()
    {
        List<ModuleAttribute> moduleAttrs = new List<ModuleAttribute>();

        //��ȡ��ǰ����������� ���������ġ��ǹ����ĺ;�̬������
        PropertyInfo[]propertyInfos=GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        //��������Ƿ��� BaseGameModule ��������
        Type baseCompType = typeof(BaseGameModule);
        for (int i = 0; i < propertyInfos.Length; i++)
        {
            PropertyInfo property = propertyInfos[i];

            //��� property.PropertyType �� BaseGameModule ���ͣ������� BaseGameModule ���κ������࣬��ô���� true
            if (!baseCompType.IsAssignableFrom(property.PropertyType))
                continue;

            //��ȡproperty���и����Ե�����
            object[] attrs = property.GetCustomAttributes(typeof(ModuleAttribute), false);
            if (attrs.Length == 0)
                continue;

            //�����������Ӷ�����Ѱ�Ҹ��������Ͷ�Ӧ�����
            Component comp = GetComponentInChildren(property.PropertyType);
            //�Ҳ��� ��¼����
            if (comp==null)
            {
                Debug.LogError($"Can't Find GameModule:{property.PropertyType}");
                continue;
            }

            //��ÿ���ҵ�������ʵ����ӵ��б���
            ModuleAttribute moduleAttr = attrs[0] as ModuleAttribute;
            moduleAttr.Module = comp as BaseGameModule;
            moduleAttrs.Add(moduleAttr);
        }

        //�������ȼ�����ģ��
        moduleAttrs.Sort((a, b) =>
        {
            return a.Priority - b.Priority;
        });

        //���ģ�鵽��Ϸ���
        for (int i = 0; i < moduleAttrs.Count; i++)
        {
            TGameFramework.Instance.AddModule(moduleAttrs[i].Module);
        }

    }



    /// <summary>
    /// ������־��Ϣ
    /// </summary>
    /// <param name="condition">��־��Ϣ���ı�����</param>
    /// <param name="stackTrace">���ǵ�����־��Ϣ�������Ķ�ջ������Ϣ</param>
    /// <param name="type">������־��Ϣ������</param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnReceiveLog(string condition, string stackTrace, LogType type)
    {
        //�ڷ�unity�༭��������ִ�У��ڹ�������Ϸ��ִ�У�
#if !UNITY_EDITOR
            if (type == LogType.Exception)//��¼�쳣����ϸ��Ϣ�Ͷ�ջ����
            {
                Debug.Log($"{condition}\n{stackTrace}");
            }
#endif
    }

    /// <summary>
    /// ��������  Ӧ�������� ���̳����� ������ͬһ����Ԫ���ϱ����Ӧ��
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ModuleAttribute : Attribute, IComparable<ModuleAttribute>
    {
        /// <summary>
        /// ���ȼ�
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// ģ��
        /// </summary>
        public BaseGameModule Module { get; set; }

        /// <summary>
        /// ��Ӹ����ԲŻᱻ����ģ��
        /// </summary>
        /// <param name="priority">���������ȼ�,��ֵԽСԽ��ִ��</param>
        public ModuleAttribute(int priority)
        {
            Priority = priority;
        }


        /// <summary>
        /// ʵ�ֽӿ�
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        int IComparable<ModuleAttribute>.CompareTo(ModuleAttribute other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }


}


