using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 定义了一个接口
/// </summary>
public interface IMessageHander
{
    /// <summary>
    /// 返回类型
    /// </summary>
    /// <returns></returns>
    Type GetHandlerType();
}

[MessageHandler]
public abstract class MessageHandler<T> : IMessageHander where T : struct
{
    public Type GetHandlerType()
    {
        return typeof(T);
    }
    /// <summary>
    /// 抽象方法  接收T参数 返回Task
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public abstract Task HandleMessage(T arg);
}

/// <summary>
/// 自定义特性   限制为类  可继承  一个类不能应用多个此特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
sealed class MessageHandlerAttribute : Attribute { }
