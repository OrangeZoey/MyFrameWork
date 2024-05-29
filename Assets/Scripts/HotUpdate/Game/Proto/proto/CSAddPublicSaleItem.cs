using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSAddPublicSaleItem : BaseProtocol
{
    public int sale_index = -1;
    public short knapsack_index = -1;
    public short item_num;
    public int gold_price;
    public int keep_time_type = 2;//0:六小时 1：12小时 2：24小时
    public int is_to_world;
    public int sale_value;
    public short sale_item_type;
    public short price_type;

    public override void Init()
    {
        this.msg_type = 4450;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(this.msg_type);
        MsgAdapter.WriteInt(this.sale_index);
        MsgAdapter.WriteShort(this.knapsack_index);
        MsgAdapter.WriteShort(this.item_num);
        MsgAdapter.WriteInt(this.gold_price);
        MsgAdapter.WriteInt(this.keep_time_type);
        MsgAdapter.WriteInt(this.is_to_world);
        MsgAdapter.WriteInt(this.sale_value);
        MsgAdapter.WriteShort(this.sale_item_type);
        MsgAdapter.WriteShort(this.price_type);
    }
    public override void Decode()
    {

    }
}
