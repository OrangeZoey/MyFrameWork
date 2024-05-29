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
    /// 资源组件
    /// </summary>
    [Module(1)]
    public static AssetModule Asset { get => TGameFramework.Instance.GetModule<AssetModule>(); }

    /// <summary>
    /// 流程组件
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

        //加载配置
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
    /// 初始化模块
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void StartupModules()
    {
        List<ModuleAttribute> moduleAttrs = new List<ModuleAttribute>();

        //获取当前类的所有属性 包括公共的、非公共的和静态的属性
        PropertyInfo[]propertyInfos=GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        //检查类型是否是 BaseGameModule 或其子类
        Type baseCompType = typeof(BaseGameModule);
        for (int i = 0; i < propertyInfos.Length; i++)
        {
            PropertyInfo property = propertyInfos[i];

            //如果 property.PropertyType 是 BaseGameModule 类型，或者是 BaseGameModule 的任何派生类，那么返回 true
            if (!baseCompType.IsAssignableFrom(property.PropertyType))
                continue;

            //获取property上有该特性的属性
            object[] attrs = property.GetCustomAttributes(typeof(ModuleAttribute), false);
            if (attrs.Length == 0)
                continue;

            //在其对象或其子对象中寻找该属性类型对应的组件
            Component comp = GetComponentInChildren(property.PropertyType);
            //找不到 记录错误
            if (comp==null)
            {
                Debug.LogError($"Can't Find GameModule:{property.PropertyType}");
                continue;
            }

            //将每个找到的特性实例添加到列表中
            ModuleAttribute moduleAttr = attrs[0] as ModuleAttribute;
            moduleAttr.Module = comp as BaseGameModule;
            moduleAttrs.Add(moduleAttr);
        }

        //根据优先级排序模块
        moduleAttrs.Sort((a, b) =>
        {
            return a.Priority - b.Priority;
        });

        //添加模块到游戏框架
        for (int i = 0; i < moduleAttrs.Count; i++)
        {
            TGameFramework.Instance.AddModule(moduleAttrs[i].Module);
        }

    }



    /// <summary>
    /// 处理日志信息
    /// </summary>
    /// <param name="condition">日志消息的文本内容</param>
    /// <param name="stackTrace">这是导致日志消息被触发的堆栈跟踪信息</param>
    /// <param name="type">这是日志消息的类型</param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnReceiveLog(string condition, string stackTrace, LogType type)
    {
        //在非unity编辑器环境下执行（在构建的游戏中执行）
#if !UNITY_EDITOR
            if (type == LogType.Exception)//记录异常的详细信息和堆栈跟踪
            {
                Debug.Log($"{condition}\n{stackTrace}");
            }
#endif
    }

    /// <summary>
    /// 控制特性  应用于属性 不继承特性 不能在同一程序元素上被多次应用
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ModuleAttribute : Attribute, IComparable<ModuleAttribute>
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// 模块
        /// </summary>
        public BaseGameModule Module { get; set; }

        /// <summary>
        /// 添加该特性才会被当作模块
        /// </summary>
        /// <param name="priority">控制器优先级,数值越小越先执行</param>
        public ModuleAttribute(int priority)
        {
            Priority = priority;
        }


        /// <summary>
        /// 实现接口
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


