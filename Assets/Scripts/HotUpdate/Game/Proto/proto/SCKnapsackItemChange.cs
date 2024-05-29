
/// <summary>
/// 物品信息变更
/// </summary>
public class SCKnapsackItemChange : BaseProtocol
{
    public short change_type;
    public short reason_type;
    public short is_bind;
    public short index;
    public ushort item_id;
    public short num;
    public uint invalid_time;

    public override void Decode()
    {
        this.change_type = MsgAdapter.ReadShort();
        this.reason_type = MsgAdapter.ReadShort();
        this.is_bind = MsgAdapter.ReadShort();
        this.index = MsgAdapter.ReadShort();
        this.item_id = MsgAdapter.ReadUShort();
        this.num = MsgAdapter.ReadShort();
        this.invalid_time = MsgAdapter.ReadUInt();
    }

    public override void Init()
    {
        base.Init();
        this.msg_type = 1501;
    }
}


