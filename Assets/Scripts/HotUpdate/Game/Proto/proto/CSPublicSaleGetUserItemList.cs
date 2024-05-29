using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//获得自己的所有拍卖物品信息
public class CSPublicSaleGetUserItemList : BaseProtocol
{
   
    public override void Init()
    {
        base.Init();
        this.msg_type = 4453;
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
