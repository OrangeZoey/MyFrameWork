using Nirvana;

public abstract class BaseProtocol
{
    protected ushort msg_type;
    public ushort MsgType { get => msg_type; }

    public virtual void Init()
    {
        this.msg_type = 0;
    }
 

    /// <summary>
    /// 编码
    /// </summary>
    public virtual void Encode()
    {
        MsgAdapter.InitWriteMsg();
    }

    /// <summary>
    /// 解码
    /// </summary>
    public abstract void Decode();


    public void EncodeAndSend(NetClient net = null)
    {
        this.Encode();
        ///发送
        MsgAdapter.Send(net);
    }

    public void Send(NetClient net = null)
    {
        MsgAdapter.Send(net);
    }




}
