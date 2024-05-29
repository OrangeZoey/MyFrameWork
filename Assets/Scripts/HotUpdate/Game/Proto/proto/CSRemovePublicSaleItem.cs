using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSRemovePublicSaleItem : BaseProtocol
{
    public int sale_index = -1;
    public override void Init()
    {
        base.Init();
        this.msg_type = 4451;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(this.msg_type);

        MsgAdapter.WriteInt(this.sale_index);
    }
    public override void Decode()
    {

    }
}
