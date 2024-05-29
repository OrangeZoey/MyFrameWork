using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SCKnapsackInfoAck : BaseProtocol
{
    // public PackageData packageData=default;
    public short max_knapsack_valid_num;
    public short max_storage_valid_num;
    public int item_count;
    public Dictionary<short, PackageInfo> info_list;
    public override void Decode()
    {
        //packageData.max_knapsack_valid_num = MsgAdapter.ReadShort();
        //packageData.max_storage_valid_num = MsgAdapter.ReadShort();
        //packageData.item_count = MsgAdapter.ReadInt();
        //packageData.info_list = new List<PackageInfo>();

        this.max_knapsack_valid_num = MsgAdapter.ReadShort();
        this.max_storage_valid_num = MsgAdapter.ReadShort();
        this.item_count = MsgAdapter.ReadInt();
        this.info_list = new Dictionary<short, PackageInfo>();

        for (int i = 0; i < this.item_count; i++)
        {
            //var info = MsgAdapter.ReadKnapsackInfo();
            //info_list.Add(info);
            PackageInfo info = new PackageInfo();
            info.item_id = MsgAdapter.ReadShort();
            info.index = MsgAdapter.ReadShort();
            info.num = MsgAdapter.ReadShort();
            info.is_bind = MsgAdapter.ReadChar();
            info.has_param = MsgAdapter.ReadChar();
            info.invalid_time = MsgAdapter.ReadUInt();
            this.info_list[info.index] = info;
        }
    }

    public override void Encode()
    {
        //MsgAdapter.WriteBegin(msg_type);
    }

    public override void Init()
    {
        base.Init();
        this.msg_type = 1500;
    }
}

public class PackageData
{
    public short max_knapsack_valid_num;
    public short max_storage_valid_num;
    public int item_count;
    public Dictionary<short, PackageInfo> info_list = new Dictionary<short, PackageInfo>();
}


public class PackageInfo
{
    public short item_id;
    public short index;
    public short num;
    public sbyte is_bind;
    public sbyte has_param;
    public uint invalid_time;

}