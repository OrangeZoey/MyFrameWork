
/// <summary>
/// 物品下架返回
/// </summary>
public class SCRemovePublicSaleItemAck : BaseProtocol
{
    public int ret;//成功返回0
    public int sale_index = -1;
    public override void Decode()
    {
        this.ret = MsgAdapter.ReadUShort();
        this.sale_index = MsgAdapter.ReadUShort();
    }

    public override void Init()
    {
        base.Init();
        this.msg_type = 9601;
    }
}


