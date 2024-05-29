
using System.Collections.Generic;

/// <summary>
/// 身上装备列表
/// </summary>
public class SCEquipList : BaseProtocol
{
    public Dictionary<int, ItemDataWrapper> equip_list;
    public FabaoInfo fabao_info;
    public int count;

    public override void Decode()
    {
        equip_list = new Dictionary<int, ItemDataWrapper>();
        MsgAdapter.ReadInt();
        MsgAdapter.ReadShort();
        fabao_info = new FabaoInfo();
        fabao_info.fabao_id = MsgAdapter.ReadUShort();
        fabao_info.fabao_gain_time = MsgAdapter.ReadUInt();
        count = MsgAdapter.ReadInt();

        for (int i = 0; i < count; i++)
        {
            int index = MsgAdapter.ReadInt();
            ItemDataWrapper equip_data = MsgAdapter.ReadItemDataWrapper();
            equip_data.index = index;
            equip_list[index] = equip_data;
        }
    }

    public override void Init()
    {
        base.Init();
        this.msg_type = 1700;
    }
}

public class FabaoInfo
{
    public ushort fabao_id;
    public uint fabao_gain_time;
}

public class ItemDataWrapper
{
    public ushort item_id;
    public short num;
    public short is_bind;
    public short has_param;
    public uint invalid_time;
    public int gold_price;
    public ItemParam param;
    public int index;
}
