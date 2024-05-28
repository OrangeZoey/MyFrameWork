using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProcedureModule : BaseGameModule
{
    
    public int value;

    //将私有字段（private fields）的属性显示在Unity编辑器中，并允许通过Inspector窗口设置和修改这些字段的值
    [SerializeField]
    private string[] proceduresNames = null;//流程名
    [SerializeField]
    private string defaultProcedureName = null;//默认流程

    //当前程序
    public BaseProcedure CurrentProcedure { get; private set; }

    //是否运行
    public bool IsRunning { get; private set; }

    //是否切换流程
    public bool IsChangingProcedure { get; private set; }

    //存储了所有已创建的流程实例，键是流程的类型，值是流程的实例
    private Dictionary<Type, BaseProcedure> procedures;
    //默认流程的实例
    private BaseProcedure defaultProcedure;
    //对象池，用于存储和管理ChangeProcedureRequest对象的创建和回收 
    private ObjectPool<ChangeProcedureRequest> changeProcedureRequestPool = new ObjectPool<ChangeProcedureRequest>(null);
    //队列，用于存储待处理的流程切换请求
    private Queue<ChangeProcedureRequest> changeProcedureQ = new Queue<ChangeProcedureRequest>();

    /// <summary>
    /// 初始化
    /// </summary>
    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();

        procedures = new Dictionary<Type, BaseProcedure>();
        bool findDefaultState = false;
        for (int i = 0; i < proceduresNames.Length; i++)
        {
            string procedureTypeName = proceduresNames[i];

            //名称不为空
            if (string.IsNullOrEmpty(procedureTypeName))
                continue;

            //获取其对应的类型，并创建该类型的实例
            //procedureTypeName 是一个字符串，它包含了你想获取的类型的完全限定名（包括其命名空间和程序集信息，如果需要的话）。Type.GetType 方法会尝试找到与此名称匹配的类型。
            // true 意味着如果找不到指定的类型，该方法将抛出一个 TypeLoadException 异常。如果你不想抛出异常而只是想在找不到类型时返回 null，你可以将这个参数设置为 false
            //Type.GetType 默认只会查找当前正在执行的程序集和已加载的引用程序集中的类型。如果类型位于其他未加载的程序集中，或者类型的名称没有包括足够的程序集限定信息，Type.GetType 可能会返回 null，即使该类型实际上存在于某个程序集中
            Type procedureType = Type.GetType(procedureTypeName, true);

            if (procedureType == null)
            {
                Debug.LogError($"Can't find procedure:`{procedureTypeName}`");
                continue;
            }
            //使用 Activator 创建 MyProcedure 的实例，并尝试转换为 BaseProcedure 类型
            BaseProcedure procedure = Activator.CreateInstance(procedureType) as BaseProcedure;

            // 布尔值记录 名称与defaultProcedureName是否相同 
            bool isDefaultState = procedureTypeName == defaultProcedureName;

            //添加进字典
            procedures.Add(procedureType, procedure);

            //如果是默认程序 设置默认程序 布尔值记录
            if (isDefaultState)
            {
                defaultProcedure = procedure;
                findDefaultState = true;
            }
        }

        //如果没有找到默认流程，打印错误信息
        if (!findDefaultState)
        {
            Debug.LogError($"You have to set a correct default procedure to start game");
        }
    }

    /// <summary>
    /// 开启模块
    /// </summary>
    protected internal override void OnModuleStart()
    {
        base.OnModuleStart();
    }

    /// <summary>
    /// 停止模块
    /// </summary>
    protected internal override void OnModuleStop()
    {
        base.OnModuleStop();

        //清空对象池
        changeProcedureRequestPool.Clear();
        //清空队列
        changeProcedureQ.Clear();
        //运行停止 设为false
        IsRunning = false;
    }

    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);
    }

    /// <summary>
    /// 开启流程
    /// </summary>
    /// <returns></returns>
    public async Task StartProcedure()
    {
        //已经开启了 直接结束
        if (IsRunning)
            return;

        //没开启 设置为true 
        IsRunning = true;

        //从 changeProcedureRequestPool 中获取一个 ChangeProcedureRequest 对象
        ChangeProcedureRequest changeProcedureRequest = changeProcedureRequestPool.Obtain();

        //TargetProcedure 属性设置为 defaultProcedure
        changeProcedureRequest.TargetProcedure = defaultProcedure;

        //将 changeProcedureRequest 对象加入 changeProcedureQ 队列中
        changeProcedureQ.Enqueue(changeProcedureRequest);

        //用 ChangeProcedureInternal 方法，并等待其完成
        //调用 await 会使当前方法等待 ChangeProcedureInternal 完成，而不会阻塞主线程
        await ChangeProcedureInternal();
    }

    /// <summary>
    /// 改变流程
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task ChangeProcedure<T>() where T : BaseProcedure
    {
        //调用 ChangeProcedure 参数为空
        await ChangeProcedure<T>(null);
    }

    /// <summary>
    /// 重载方法 ChangeProcedure<T>(object value)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task ChangeProcedure<T>(object value) where T : BaseProcedure
    {
        //程序没有在运行 结束
        if (!IsRunning)
            return;

        //从 procedures 字典中根据类型 T 获取对应的过程 procedure
        if (!procedures.TryGetValue(typeof(T),out BaseProcedure procedure))
        {
            //输出一条错误日志并返回
            Debug.Log($"Change Procedure Failed, Can't find Proecedure:${typeof(T).FullName}");
            return;
        }

        //创建请求对象
        ChangeProcedureRequest changeProcedureRequest = changeProcedureRequestPool.Obtain();
        //赋值
        changeProcedureRequest.TargetProcedure = procedure;
        changeProcedureRequest.Value = value;
        changeProcedureQ.Enqueue(changeProcedureRequest);

        //如果当前没有正在改变的过程 调用方法
        if (!IsChangingProcedure)
        {
            await ChangeProcedureInternal();
        }
    }

    private async Task ChangeProcedureInternal()
    {
        //有正在改变的 结束
        if (IsChangingProcedure)
            return;

        IsChangingProcedure = true;
        //进入循环 处理每一个请求
        while (changeProcedureQ.Count>0)
        {
            ChangeProcedureRequest request = changeProcedureQ.Dequeue();
            //检查请求和请求的目标过程是否为 null
            if (request == null || request.TargetProcedure == null)
                continue;

            //如果当前有程序正在运行
            if (CurrentProcedure != null)
            {
                //调用当前程序的 OnLeaveProcedure 方法
                await CurrentProcedure.OnLeaveProcedure();
            }

            //将当前程序设置为请求的目标程序
            CurrentProcedure = request.TargetProcedure;
            //并调用 OnEnterProcedure方法
            await CurrentProcedure.OnEnterProcedure(request.Value);
        }

        //处理完所有请求后，它将 IsChangingProcedure 设置回 false。
        IsChangingProcedure = false;
    }
}

/// <summary>
/// 改变程序请求
/// </summary>
public class ChangeProcedureRequest
{
    public BaseProcedure TargetProcedure { get; set; }
    public object Value { get; set; }
}