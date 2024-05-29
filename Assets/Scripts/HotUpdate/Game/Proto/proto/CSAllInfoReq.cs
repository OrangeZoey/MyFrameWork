using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSAllInfoReq : BaseProtocol
{
    public int no_chat_record=default;
    public override void Init()
    {
        base.Init();
        this.msg_type = 1454;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(this.msg_type);

        MsgAdapter.WriteInt(this.no_chat_record);
    }
    public override void Decode()
    {
        
    }
}
