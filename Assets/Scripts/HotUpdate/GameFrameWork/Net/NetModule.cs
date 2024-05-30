using Nirvana;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Nirvana.NetClient;

/// <summary>
/// 链接状态
/// </summary>
public enum ConnectState : byte
{
    Connected, Disconnect, Connecting
}

/// <summary>
/// 服务器类型
/// </summary>
public enum ServerEnum : byte
{
    LoginServer, GameServer, None
}


public class NetModule : BaseGameModule
{
    //loginServer 的IP和端口号
    public string loginServerIP;
    public int loginServerPort;

    //游戏服务器的IP和端口号
    public string gameServerIP;
    public int gameServerPort;

    //连接的客户端
    private NetClient _curNetClient;

    //当前服务器类型
    private ServerEnum _curServerEnum;

    /// <summary>
    /// 当前的网络连接状态
    /// </summary>
    private ConnectState _crtConnectState;
    public ConnectState CrtConnectState { get => _crtConnectState; }

    /// <summary>
    /// 注册消息的操作。
    /// </summary>
    private Dictionary<ushort, List<Action<BaseProtocol>>> msg_operate_table;//存储与特定消息类型相关联的消息处理函数列表
    private Dictionary<ushort, Type> msg_type_map; //存储消息类型和协议类型之间的映射关系

    public bool networkRun;

    private ReceiveDelegate receiveEvt; //用于处理接收到的网络消息或数据的委托
    private DisconnectDelegate disEvt; //用于处理网络断开事件的委托

    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();

        //服务器IP和端口号

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
        //获取消息处理列表
        if (this.msg_operate_table.TryGetValue(msg_type, out List<Action<BaseProtocol>> actList))
        {
            //找到了 添加进去
            actList.Add(msg_oper_func);
        }
        else
        {
            //没找到  添加新的进去
            List<Action<BaseProtocol>> lst = new List<Action<BaseProtocol>>();
            lst.Add(msg_oper_func);
            this.msg_operate_table[msg_type] = lst;
        }
    }

    /// <summary>
    /// 注册特定类型的协议  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="msg_oper_func"></param>
    public void RegisterProtocol<T>(Action<BaseProtocol> msg_oper_func) where T : BaseProtocol, new()
    {
        //返回消息类型
        ushort msg_type = ProtocolPool.Instance.Register<T>();

        //注册失败
        if (msg_type <= 0)
            return;

        //存储
        msg_type_map[msg_type] = typeof(T);
        //将消息类型 msg_type 和消息处理函数 msg_oper_func 关联起来
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

    /// <summary>
    /// 停止
    /// </summary>
    protected internal override void OnModuleStop()
    {
        base.OnModuleStop();
        //停止运行
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
    /// <summary>
    /// 异步连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public async Task AsyncConnect(string ip, int port)
    {
        //检查网络连接状态
        if (!networkRun)
            return;

        ServerEnum tmpServerEnum = this._curServerEnum;
        //当前状态不是Disconnect 断开之前的连接
        if (this.CrtConnectState != ConnectState.Disconnect)
        {
            UnityLog.Info("正在断开之前的链接.....");
            this.Disconect();
        }

        //状态设为连接中
        _crtConnectState = ConnectState.Connecting;

        //创建NetClient
        this._curNetClient = new NetClient();
        //连接
        this._curNetClient.Connect(ip, port, async (is_succ) =>
        {
            //连接成功
            if (is_succ)
            {
                UnityLog.Info("开始成功");
                //设置状态
                _crtConnectState = ConnectState.Connected;
                UnityLog.Info($"Async Connect to  {tmpServerEnum} server Ret: status " + is_succ);
                //开始接收 来自服务器的数据
                this._curNetClient.StartReceive();

                //定义事件处理器 处理接收数据和断开连接的事件
                receiveEvt = (byte[] message, uint length) => { NetClient_ReceiveEvent(message, length, this._curNetClient, tmpServerEnum); };
                disEvt = () => { _netClient_DisconnectEvent(this._curNetClient, tmpServerEnum); };

                //添加到事件中
                this._curNetClient.ReceiveEvent += receiveEvt;
                this._curNetClient.DisconnectEvent += disEvt;

                //发送消息
                GameManager.Message.Post<MessageType.NetConnected>(new MessageType.NetConnected() { serverEnum = tmpServerEnum }).Coroutine();

                await Task.Yield();
            }
            else
            {
                //连接失败
                UnityLog.Warn("Connect Failed!!!!!!!");
                //设置状态
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
        //检查连接状态
        if (_crtConnectState == ConnectState.Connected)
        {
            //检查网络连接对象
            if (_curNetClient != null)
            {
                //断开连接
                _curNetClient.Disconnect();
            }
        }

    }
    #endregion

    /// <summary>
    /// 客户端断开连接
    /// </summary>
    /// <param name="netClient"></param>
    /// <param name="serverEn"></param>
    private void _netClient_DisconnectEvent(NetClient netClient, ServerEnum serverEn)
    {
        UnityLog.Info($"net disconnected ---> {serverEn}");

        //更新状态
        _crtConnectState = ConnectState.Disconnect;

        //移除事件监听器
        if (receiveEvt != null)
            netClient.ReceiveEvent -= receiveEvt;

        if (disEvt != null)
            netClient.DisconnectEvent -= disEvt;

        //发布消息
        GameManager.Message.Post<MessageType.NetDisconnected>(new MessageType.NetDisconnected() { serverEnum = serverEn }).Coroutine();
    }

    /// <summary>
    /// 收到的数据事件
    /// </summary>
    /// <param name="message">收到的数据</param>
    /// <param name="length">实际收到的长度</param>
    /// <param name="netClient">当前的网络客户端连接</param>
    /// <param name="serverEnum">服务器类型</param>
    private void NetClient_ReceiveEvent(byte[] message, uint length, NetClient netClient, ServerEnum serverEnum)
    {
        MsgAdapter.InitReadMsg(message);
        //读取消息类型（id），需要根据消息类型到协议池子中找到协议
        ushort msgType = MsgAdapter.ReadUShort();
        MsgAdapter.ReadUShort();
        UnityEngine.Debug.Log("收到数据长度 = " + message.Length + "  lenght = " + length + "    msgType = " + msgType);

        //查找消息对应的处理
        if (this.msg_operate_table.TryGetValue(msgType, out List<Action<BaseProtocol>> actions))
        {
            //获取协议
            var proto = ProtocolPool.Instance.GetProtocolByType(msgType);
            //不为空 解码
            if (proto != null)
            {
                proto.Decode();
                //执行对应的操作
                foreach (var act in actions)
                {
                    act?.Invoke(proto);
                }
            }
            else //为空 没找到
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
