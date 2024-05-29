using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSBuyPublicSaleItem : BaseProtocol
{
    public int seller_uid;
    public int sale_index = -1;
    public int item_id;
    public int item_num;
    public int gold_price;
    public int sale_value;
    public short sale_item_type;
    public short price_type;

    public override void Init()
    {
        this.msg_type = 4452;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(this.msg_type);

        MsgAdapter.WriteInt(this.seller_uid);
        MsgAdapter.WriteInt(this.sale_index);
        MsgAdapter.WriteInt(this.item_id);
        MsgAdapter.WriteInt(this.item_num);
        MsgAdapter.WriteInt(this.gold_price);
        MsgAdapter.WriteInt(this.sale_value);
        MsgAdapter.WriteShort(this.sale_item_type);
        MsgAdapter.WriteShort(this.price_type);
    }
    public override void Decode()
    {

    }
}
