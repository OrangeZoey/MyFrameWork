using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSTakeOffEquip : BaseProtocol
{
    public int index;

    public override void Init()
    {
        this.msg_type = 1751;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(this.msg_type);
        MsgAdapter.WriteInt(index);
    }
    public override void Decode()
    {

    }
}
