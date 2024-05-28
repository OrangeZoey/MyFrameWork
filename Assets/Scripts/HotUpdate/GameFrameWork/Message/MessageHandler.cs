using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ������һ���ӿ�
/// </summary>
public interface IMessageHander
{
    /// <summary>
    /// ��������
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
    /// ���󷽷�  ����T���� ����Task
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public abstract Task HandleMessage(T arg);
}

/// <summary>
/// �Զ�������   ����Ϊ��  �ɼ̳�  һ���಻��Ӧ�ö��������
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
sealed class MessageHandlerAttribute : Attribute { }
