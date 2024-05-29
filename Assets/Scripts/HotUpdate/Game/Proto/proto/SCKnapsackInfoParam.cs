using System.Collections.Generic;

public class SCKnapsackInfoParam : BaseProtocol
{
    public Dictionary<short, ItemInfoList> info_list = new Dictionary<short, ItemInfoList>();
    public int count;

    public override void Init()
    {
        base.Init();
        this.msg_type = 1502;
    }

    public override void Decode()
    {
        this.count = MsgAdapter.ReadInt();

        for (int i = 0; i < count; i++)
        {
            ItemInfoList infoList = new ItemInfoList();
            infoList.index = MsgAdapter.ReadShort();
            infoList.reserve = MsgAdapter.ReadShort();
            infoList.param = MsgAdapter.ReadItemParamData();
            info_list[infoList.index] = infoList;
        }
    }
}

public class Item
{
    public Dictionary<short, ItemInfoList> info_list = new Dictionary<short, ItemInfoList>();
    public int count;

}



public class ItemInfoList
{
    public short index;
    public short reserve;
    public ItemParam param;
}


public class ItemParam
{
    public short quality;
    public short strengthen_level;
    public short shen_level;
    public short fuling_level;
    public short star_level;
    public short has_lucky;
    public short fumo_id;
    public List<ushort> xianpin_type_list;
    public int param1;
    public int param2;
    public int param3;
    public byte rand_attr_type_1;
    public byte rand_attr_type_2;
    public byte rand_attr_type_3;
    public byte reserve_type;
    public ushort rand_attr_val_1;
    public ushort rand_attr_val_2;
    public ushort rand_attr_val_3;
    public ushort reserve;
}


