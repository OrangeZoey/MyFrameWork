using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TGameFramework
{
    //静态实例  只有一个
    public static TGameFramework Instance { get; private set; }

    //是否初始化
    public static bool Initialized { get; private set; }

    //存储游戏模块
    private Dictionary<Type, BaseGameModule> m_modules = new Dictionary<Type, BaseGameModule>();


    public static void Initialize()
    {
        Instance = new TGameFramework();
    }

    /// <summary>
    /// 获取游戏模块
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetModule<T>() where T : BaseGameModule
    {
        //找到 返回该模块
        //否则 返回默认值
        if (m_modules.TryGetValue(typeof(T), out BaseGameModule module))
        {
            return module as T;
        }

        return default(T);
    }

    /// <summary>
    /// 添加模块
    /// </summary>
    /// <param name="module"></param>
    public void AddModule(BaseGameModule module)
    {
        Type moduleType = module.GetType();
        if (m_modules.ContainsKey(moduleType))
        {
            Debug.Log($"Module添加失败，重复:{moduleType.Name}");
            return;
        }

        m_modules.Add(moduleType, module);
    }

    #region MyRegion
    //检查是否已经有一个TGameFramework的实例在运行，如果有，则禁用当前的游戏对象。
    //加载配置信息，这可能包括从文件或资源包中读取配置数据。
    //初始化全局配置和其他配置。
    //        private IEnumerator Start()
    //        {
    //            if (Instance != null)
    //            {
    //                gameObject.SetActive(false);
    //                yield break;
    //                //return;
    //
    //            UnityLog.Info("===游戏启动===");
    //            Instance = this;
    //            DontDestroyOnLoad(gameObject);
    //            UnityLog.Info("===>加载配置");
    //#if UNITY_EDITOR
    //            string path = "Assets/Datas/Config";
    //            ConfigManager.LoadAllConfigsByFile(path);
    //            //yield return ConfigManager.LoadAllConfigsByFile(path);
    //#else
    //            string path = $"{Application.streamingAssetsPath}/AssetBundles";
    //            string subFolder = $"Datas/Config";
    //            //ConfigManager.LoadAllConfigsByBundle(path, subFolder);
    //            yield return ConfigManager.LoadAllConfigsByBundle(path, subFolder);
    //#endif
    //            GlobalConfig.InitGlobalConfig();
    //            BuffConfig.ParseConfig();
    //            SkillConfig.ParseConfig();
    //            SkillRuneConfig.ParseConfig();
    //            BulletConfig.ParseConfig();
    //            SpellFieldConfig.ParseConfig();
    //            LevelConfig.ParseConfig();
    //            WeaponConfig.ParseConfig();
    //            DialogConfig.ParseConfig();
    //            BlessingConfig.ParseConfig();
    //            SpawnMonsterConfig.ParseConfig();
    //            I18NConfig.ParseConfig();
    //            GameEffectHandler.RegisterHandlers();
    //            UnityLog.Info("<===配置加载完毕");
    //
    //            UnityLog.Info("===>初始化模块组件");
    //            Instance.InitModules();
    //            UnityLog.Info("<===模块组件初始化完毕");
    //            UnityLog.Info("===>启动模块组件");
    //            Instance.StartModules();
    //            UnityLog.Info("<===模块组件已就绪");
    //        }

    #endregion

    public void Update()
    {
        //是否初始化
        if (!Initialized)
            return;

        //模块字典是否为空
        if (m_modules == null)
            return;

        if (!Initialized)
            return;

        float delaTime = UnityEngine.Time.deltaTime;
        foreach (var module in m_modules.Values)
        {
            module.OnModuleUpdate(delaTime);
        }
    }

    public void LateUpdate()
    {
        //是否初始化
        if (!Initialized)
            return;

        //模块字典是否为空
        if (m_modules == null)
            return;

        if (!Initialized)
            return;

        float deltaTime = UnityEngine.Time.deltaTime;
        foreach (var module in m_modules.Values)
        {
            module.OnModuleLateUpdate(deltaTime);
        }
    }

    public void FixedUpdate()
    {
        if (!Initialized)
            return;

        if (m_modules == null)
            return;

        if (!Initialized)
            return;

        float deltaTime = UnityEngine.Time.fixedDeltaTime;
        foreach (var module in m_modules.Values)
        {
            module.OnModuleFixedUpdate(deltaTime);
        }
    }

    /// <summary>
    /// 初始化模块
    /// </summary>
    public void InitModules()
    {
        //初始化过了
        if (Initialized)
            return;

        Initialized = true;
        //StartupModules();

        //所有模块初始化
        foreach (var module in m_modules.Values)
        {
            module.OnModuleInit();
        }
    }

    /// <summary>
    /// 启动模块
    /// </summary>
    public void StartModules()
    {
        if (m_modules == null)
            return;

        if (!Initialized)
            return;

        foreach (var module in m_modules.Values)
        {
            module.OnModuleStart();
        }
    }

    /// <summary>
    /// 销毁模块
    /// </summary>
    public void Destroy()
    {
        if (!Initialized)
            return;

        if (Instance != this)
            return;

        if (Instance.m_modules == null)
            return;

        foreach (var module in Instance.m_modules.Values)
        {
            module.OnModuleStop();
        }

        //Destroy(Instance.gameObject);
        Instance = null;
        Initialized = false;

    }
}
