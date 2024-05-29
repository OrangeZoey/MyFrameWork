
/// <summary>
/// 购买物品返回
/// </summary>
public class SCBuyPublicSaleItemAck : BaseProtocol
{
    public int ret;//成功返回0
    public int seller_uid = -1;
    public int sale_index = -1;
    public override void Decode()
    {
        this.ret = MsgAdapter.ReadUShort();
        this.seller_uid = MsgAdapter.ReadUShort();
        this.sale_index = MsgAdapter.ReadUShort();
    }

    public override void Init()
    {
        base.Init();
        this.msg_type = 9602;
    }
}


