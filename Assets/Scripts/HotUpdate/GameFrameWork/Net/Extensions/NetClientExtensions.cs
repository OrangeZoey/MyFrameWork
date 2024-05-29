//------------------------------------------------------------------------------
// This file is part of CrossGate project in Area6.
// Copyright © 2013-2015 Area6 Technology Co., Ltd.
// All Right Reserved.
//------------------------------------------------------------------------------

///using LuaInterface;
using Nirvana;

/// <summary>
/// The extension for <see cref="NetClient"/>
/// </summary>
public static class NetClientExtensions
{
    //public delegate void ReceiveMessageDelegate(LuaByteBuffer message);

    //public static NetClient.ReceiveDelegate ListenMessage(this NetClient client, ReceiveMessageDelegate receiveDelegate)
    //{
    //    NetClient.ReceiveDelegate handle = delegate (byte[] bytes, uint length)
    //    {
    //        UnityEngine.Debug.Log("收到数据长度 = " + bytes.Length + "  lenght = " + length);
    //        var message = new LuaByteBuffer(bytes, (int)length);
    //        receiveDelegate(message);
    //    };

    //    client.ReceiveEvent += handle;
    //    return handle;
    //}

    //public static void UnlistenMessage(this NetClient client, NetClient.ReceiveDelegate handle)
    //{
    //    client.ReceiveEvent -= handle;
    //}

    //public static NetClient.DisconnectDelegate ListenDisconnect(this NetClient client, NetClient.DisconnectDelegate handler)
    //{
    //    client.DisconnectEvent += handler;
    //    return handler;
    //}

    //public static void UnlistenDisconnect(
    //    this NetClient client, NetClient.DisconnectDelegate handle)
    //{
    //    client.DisconnectEvent -= handle;
    //}
}
