
using System.Collections.Generic;

/// <summary>
/// 获取自己出售物品列表
/// </summary>
public class SCGetPublicSaleItemListAck : BaseProtocol
{
    public int count;

    public List<SaleItem> saleitem_list;

    public override void Decode()
    {
        this.count = MsgAdapter.ReadInt();

        saleitem_list = new List<SaleItem>();

      

        for (int i = 0; i < this.count; i++)
        {
           
            SaleItem saleitem = new SaleItem();
            saleitem.item_id = MsgAdapter.ReadUShort();
            saleitem.num = MsgAdapter.ReadShort();
            saleitem.is_bind = MsgAdapter.ReadShort();
            saleitem.has_param = MsgAdapter.ReadShort();
            saleitem.invalid_time = MsgAdapter.ReadUInt();
            saleitem.gold_price = MsgAdapter.ReadInt();
            saleitem.param = MsgAdapter.ReadItemParamData();

           
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
        this.msg_type = 9603;
    }
}


