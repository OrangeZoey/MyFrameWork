
/// <summary>
/// 使用成功
/// </summary>
public class SCUseItemSuc : BaseProtocol
{
    public ushort item_id;
    public override void Decode()
    {      
        this.item_id = MsgAdapter.ReadUShort();
    }

    public override void Init()
    {
        base.Init();
        this.msg_type = 1504;
    }
}


