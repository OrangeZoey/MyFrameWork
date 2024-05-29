using Nirvana;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Nirvana.NetClient;

public enum ConnectState : byte
{
    Connected, Disconnect, Connecting
}

public enum ServerEnum : byte
{
    LoginServer, GameServer, None
}


public class NetModule : BaseGameModule
{
    public string loginServerIP;
    public int loginServerPort;

    public string gameServerIP;
    public int gameServerPort;

    private NetClient _curNetClient;

    private ServerEnum _curServerEnum;
    /// <summary>
    /// 当前的网络连接状态
    /// </summary>
    private ConnectState _crtConnectState;
    public ConnectState CrtConnectState { get => _crtConnectState; }

    /// <summary>
    /// 注册消息的操作。
    /// </summary>
    private Dictionary<ushort, List<Action<BaseProtocol>>> msg_operate_table;
    private Dictionary<ushort, Type> msg_type_map;

    public bool networkRun;

    private ReceiveDelegate receiveEvt;
    private DisconnectDelegate disEvt;

    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();

        //loginServerIP = "10.161.23.56";
        //loginServerIP = "10.161.29.53";
        loginServerIP = "10.161.29.119";
       // loginServerIP = "127.0.0.1";
        loginServerPort = 10300;

        //gameServerIP = "10.161.23.56";
        //gameServerIP = "10.161.29.53";
        gameServerIP = "10.161.29.119";
        //gameServerIP = "127.0.0.1";
        gameServerPort = 4777;

        _curServerEnum = ServerEnum.None;
        ///协议池初始化
        ProtocolPool.Instance.Init();


        msg_operate_table = new Dictionary<ushort, List<Action<BaseProtocol>>>();
        msg_type_map = new Dictionary<ushort, Type>();

        _crtConnectState = ConnectState.Disconnect;
    }
    protected internal override void OnModuleFixedUpdate(float deltaTime)
    {
        base.OnModuleFixedUpdate(deltaTime);
    }

    /// <summary>
    ///  -- 注册协议处理函数
    /// </summary>
    /// <param name="msg_type"></param>
    /// <param name="msg_oper_func"></param>
    public void RegisterMsgOperate(ushort msg_type, Action<BaseProtocol> msg_oper_func)
    {
        if (this.msg_operate_table.TryGetValue(msg_type, out List<Action<BaseProtocol>> actList))
        {
            actList.Add(msg_oper_func);
        }
        else
        {
            List<Action<BaseProtocol>> lst = new List<Action<BaseProtocol>>();
            lst.Add(msg_oper_func);
            this.msg_operate_table[msg_type] = lst;
        }
    }

    public void RegisterProtocol<T>(Action<BaseProtocol> msg_oper_func) where T : BaseProtocol, new()
    {
        ushort msg_type = ProtocolPool.Instance.Register<T>();
        if (msg_type <= 0)
            return;
        msg_type_map[msg_type] = typeof(T);
        this.RegisterMsgOperate(msg_type, msg_oper_func);
    }

    /// <summary>
    /// 取消注册协议
    /// </summary>
    /// <param name="msg_type">协议编号</param>
    public void UnRegisterMsgOperate(ushort msg_type)
    {
        if (this.msg_operate_table.TryGetValue(msg_type, out List<Action<BaseProtocol>> actList))
        {
            this.msg_operate_table[(ushort)msg_type].Clear();
            this.msg_operate_table.Remove(msg_type);
        }
    }


    protected internal override void OnModuleLateUpdate(float deltaTime)
    {
        base.OnModuleLateUpdate(deltaTime);
    }

    protected internal override void OnModuleStart()
    {
        base.OnModuleStart();
    }

    protected internal override void OnModuleStop()
    {
        base.OnModuleStop();
        networkRun = false;

        foreach (var item in msg_type_map)
        {
            ProtocolPool.Instance.UnRegister(item.Value, item.Key);
            this.UnRegisterMsgOperate(item.Key);
        }
    }

    protected internal override void OnModuleUpdate(float deltaTime)
    {
        base.OnModuleUpdate(deltaTime);
    }

    #region 连接和断开
    public async Task AsyncConnect(string ip, int port)
    {
        if (!networkRun)
            return;

        ServerEnum tmpServerEnum = this._curServerEnum;
        if (this.CrtConnectState != ConnectState.Disconnect)
        {
            UnityLog.Info("正在断开之前的链接.....");
            this.Disconect();
        }

        _crtConnectState = ConnectState.Connecting;

        this._curNetClient = new NetClient();

        this._curNetClient.Connect(ip, port, async (is_succ) =>
        {
            if (is_succ)
            {
                UnityLog.Info("开始成功");
                _crtConnectState = ConnectState.Connected;
                UnityLog.Info($"Async Connect to  {tmpServerEnum} server Ret: status " + is_succ);
                this._curNetClient.StartReceive();

                receiveEvt = (byte[] message, uint length) => { NetClient_ReceiveEvent(message, length, this._curNetClient, tmpServerEnum); };
                disEvt = () => { _netClient_DisconnectEvent(this._curNetClient, tmpServerEnum); };

                this._curNetClient.ReceiveEvent += receiveEvt;
                this._curNetClient.DisconnectEvent += disEvt;

                GameManager.Message.Post<MessageType.NetConnected>(new MessageType.NetConnected() { serverEnum = tmpServerEnum }).Coroutine();

                await Task.Yield();
            }
            else
            {
                UnityLog.Warn("Connect Failed!!!!!!!");
                _crtConnectState = ConnectState.Disconnect;
            }

        });

        await Task.Yield();
    }

    /// <summary>
    /// 断开和服务器的链接
    /// </summary>
    public void Disconect()
    {
        if (_crtConnectState == ConnectState.Connected)
        {
            if (_curNetClient != null)
            {
                _curNetClient.Disconnect();
            }
        }

    }
    #endregion


    private void _netClient_DisconnectEvent(NetClient netClient, ServerEnum serverEn)
    {
        UnityLog.Info($"net disconnected ---> {serverEn}");

        _crtConnectState = ConnectState.Disconnect;

        if (receiveEvt != null)
            netClient.ReceiveEvent -= receiveEvt;

        if (disEvt != null)
            netClient.DisconnectEvent -= disEvt;

        GameManager.Message.Post<MessageType.NetDisconnected>(new MessageType.NetDisconnected() { serverEnum = serverEn }).Coroutine();
    }

    private void NetClient_ReceiveEvent(byte[] message, uint length, NetClient netClient, ServerEnum serverEnum)
    {
        MsgAdapter.InitReadMsg(message);
        ///读取消息类型（id），需要根据消息类型到协议池子中找到协议
        ushort msgType = MsgAdapter.ReadUShort();
        MsgAdapter.ReadUShort();
        UnityEngine.Debug.Log("收到数据长度 = " + message.Length + "  lenght = " + length + "    msgType = " + msgType);

        if (this.msg_operate_table.TryGetValue(msgType, out List<Action<BaseProtocol>> actions))
        {
            var proto = ProtocolPool.Instance.GetProtocolByType(msgType);
            if (proto != null)
            {
                proto.Decode();
                foreach (var act in actions)
                {
                    act?.Invoke(proto);
                }
            }
            else
            {
                UnityLog.Info($"Unknow protocol:[{msgType}]");
            }
        }

    }




    public void SendMsg(BaseProtocol proto)
    {
        if (!networkRun || proto == null)
            return;

        proto.EncodeAndSend();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public NetClient GetCurNetClient()
    {
        return _curNetClient;
    }

    /// <summary>
    /// 连接到登录服务器
    /// </summary>
    /// <returns></returns>
    public async Task ConnectLoginServer()
    {
        this._curServerEnum = ServerEnum.LoginServer;
        await this.AsyncConnect(this.loginServerIP, this.loginServerPort);
    }


    /// <summary>
    /// 链接游戏服务器
    /// </summary>
    /// <returns></returns>
    public async Task ConnectGameServer()
    {
        this._curServerEnum = ServerEnum.GameServer;
        await this.AsyncConnect(this.gameServerIP, this.gameServerPort);
    }


}
