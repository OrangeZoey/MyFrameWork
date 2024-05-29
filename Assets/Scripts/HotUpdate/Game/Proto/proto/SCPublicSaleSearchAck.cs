
using System.Collections.Generic;

/// <summary>
/// 搜索返回
/// </summary>
public class SCPublicSaleSearchAck : BaseProtocol
{
    public int cur_page;
    public int total_page;
    public int count;

    public List<SaleItem> saleitem_list;

    public override void Decode()
    {
        this.cur_page = MsgAdapter.ReadInt();
        this.total_page = MsgAdapter.ReadInt();
        this.count = MsgAdapter.ReadInt();
        saleitem_list = new List<SaleItem>();

        int seller_uid = 0;
        string seller_name = "";

        for (int i = 0; i < this.count; i++)
        {
            seller_uid = MsgAdapter.ReadInt();
            seller_name = MsgAdapter.ReadStrN(32);

            SaleItem saleitem = new SaleItem();
            saleitem.item_id = MsgAdapter.ReadUShort();
            saleitem.num = MsgAdapter.ReadShort();
            saleitem.is_bind = MsgAdapter.ReadShort();
            saleitem.has_param = MsgAdapter.ReadShort();
            saleitem.invalid_time = MsgAdapter.ReadUInt();
            saleitem.gold_price = MsgAdapter.ReadInt();
            saleitem.param = MsgAdapter.ReadItemParamData();

            saleitem.seller_uid = seller_uid;
            saleitem.seller_name = seller_name;
            saleitem.sale_index = MsgAdapter.ReadInt();
            saleitem.sale_type = MsgAdapter.ReadInt();
            saleitem.level = MsgAdapter.ReadShort();
            saleitem.prof = MsgAdapter.ReadShort();
            saleitem.color = MsgAdapter.ReadShort();
            saleitem.order = MsgAdapter.ReadShort();
            saleitem.gold_price = MsgAdapter.ReadInt();
            saleitem.sale_value = MsgAdapter.ReadInt();
            saleitem.price_type = MsgAdapter.ReadShort();
            saleitem.sale_item_type = MsgAdapter.ReadShort();
            saleitem.sale_time = MsgAdapter.ReadUInt();
            saleitem.due_time = MsgAdapter.ReadUInt();

            saleitem_list.Add(saleitem);
        }

    }

    public override void Init()
    {
        base.Init();
        this.msg_type = 9604;
    }
}

public class SaleItem
{
    public ushort item_id;
    public short num;
    public short is_bind;
    public short has_param;
    public uint invalid_time;
    public int gold_price;
    public ItemParam param;
    public int seller_uid;
    public string seller_name;
    public int sale_index;
    public int sale_type;
    public short level;
    public short prof;
    public short color;
    public short order;
    public int sale_value;
    public short price_type;
    public short sale_item_type;
    public uint sale_time;
    public uint due_time;
}
