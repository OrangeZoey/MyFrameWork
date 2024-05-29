using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSKnapsackStoragePutInOrder : BaseProtocol
{
    public short is_storage;    //整理的是哪个，1为仓库，0为背包
    public short ignore_bind;   //是否忽略绑定，1为是，0为否
   
    public override void Init()
    {
        base.Init();
        msg_type = 1554;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(msg_type);
        MsgAdapter.WriteShort(is_storage);
        MsgAdapter.WriteShort(ignore_bind);       
        MsgAdapter.WriteShort(0);
    }
    public override void Decode()
    {

    }
}
