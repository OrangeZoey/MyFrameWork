using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSUseItem : BaseProtocol
{
    public short index;
    public short num;
    public short equip_index;
    public override void Init()
    {
        base.Init();
        msg_type = 1551;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(msg_type);
        MsgAdapter.WriteShort(index);
        MsgAdapter.WriteShort(num);
        MsgAdapter.WriteShort(equip_index);
        MsgAdapter.WriteShort(0);
    }
    public override void Decode()
    {

    }
}
