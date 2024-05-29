using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSRoleInfoReq : BaseProtocol
{
    public override void Init()
    {
        this.msg_type = 1450;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(this.msg_type);
    }
    public override void Decode()
    {
        
    }
}
