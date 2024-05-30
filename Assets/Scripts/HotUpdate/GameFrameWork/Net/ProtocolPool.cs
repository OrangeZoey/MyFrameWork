using Nirvana;
using System;
using System.Collections.Generic;

/// <summary>
/// /-- 协议池  
/// </summary>
public class ProtocolPool : Singleton<ProtocolPool>
{
    public Dictionary<Type, BaseProtocol> protocol_list; //按类型存储协议对象
    public Dictionary<ushort, BaseProtocol> protocol_list_by_type; //根据消息号存储协议

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        protocol_list = new Dictionary<Type, BaseProtocol>();
        protocol_list_by_type = new Dictionary<ushort, BaseProtocol>();
    }

    /// <summary>
    /// 删除
    /// </summary>
    public void Delete()
    {
        protocol_list.Clear();
        protocol_list_by_type.Clear();
        protocol_list = null;
        protocol_list_by_type = null;
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ushort Register<T>() where T : BaseProtocol, new()
    {
        //从protocol_list字典中获取
        BaseProtocol proto = AddProtocol<T>();

        //不为空 添加进字典
        if (proto != null)
        {
            protocol_list_by_type[proto.MsgType] = proto;
            //返回消息号
            return proto.MsgType;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 通过类型注销与协议相关的信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void UnRegister<T>() where T : BaseProtocol, new()
    {
        //获取类型
        Type type = typeof(T);
        //获取协议
        if (protocol_list.TryGetValue(type, out BaseProtocol protocol))
        {
            //移除
            protocol_list.Remove(type);
            protocol_list_by_type.Remove(protocol.MsgType);
        }
    }

    /// <summary>
    /// 通过协议号注销
    /// </summary>
    /// <param name="type"></param>
    /// <param name="msgType"></param>
    public void UnRegister(Type type,ushort msgType)
    {
        if (protocol_list_by_type.TryGetValue(msgType, out BaseProtocol protocol))
        {
            protocol_list.Remove(type);
            protocol_list_by_type.Remove(protocol.MsgType);
        }
    }

    /// <summary>
    /// 获取协议
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public BaseProtocol GetProtocol<T>() where T : BaseProtocol, new()
    {
        //协议列表中存在
        if (protocol_list.TryGetValue(typeof(T), out BaseProtocol protocol))
        {
            //初始化 返回
            protocol.Init();
            return protocol;
        }
        else
        {
            //不存在 创建新的
            protocol = AddProtocol<T>();
            //创建成功 初始化
            if (protocol != null)
            {
                protocol.Init();
            }
            return protocol;
        }

    }

    /// <summary>
    /// 通过msgType获取协议
    /// </summary>
    /// <param name="msgType"></param>
    /// <returns></returns>
    public BaseProtocol GetProtocolByType(ushort msgType)
    {
        if (protocol_list_by_type.TryGetValue(msgType, out BaseProtocol protocol))
        {
            protocol.Init();
            return protocol;
        }
        else
        {
            UnityLog.Warn($"msgType = {msgType} has not  Registed");
            return null;
        }

    }

    /// <summary>
    /// 用于添加或获取一个类型为 T 的 BaseProtocol 对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public BaseProtocol AddProtocol<T>() where T : BaseProtocol, new()
    {
        //从字典中获取
        if (protocol_list.TryGetValue(typeof(T), out BaseProtocol protocol))
        {
            return protocol;
        }

        //字典中没有 创建一个
        BaseProtocol proto = new T();
        //初始化
        proto.Init();
        //添加到字典
        protocol_list[typeof(T)] = proto;
        return proto;
    }
}

