
/// <summary>
/// --进入场景返回
/// </summary>
public class SCUserEnterGSAck : BaseProtocol
{
    public int result;
    public override void Decode()
    {
        this.result = MsgAdapter.ReadInt();
    }

    public override void Init()
    {
        base.Init();
        this.msg_type = 1000;
    }
}

 
