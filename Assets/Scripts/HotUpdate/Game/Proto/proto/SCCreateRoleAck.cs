using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCCreateRoleAck : BaseProtocol
{
    public int result ;
    public int role_id ;
    public string role_name;
    public sbyte avatar ;
    public sbyte sex ;
    public sbyte prof ;
    public sbyte camp_type;
    public int level ;
    public uint create_time ;
    public override void Init()
    {
        base.Init();
        this.msg_type = 7100;

    }
    public override void Decode()
    {
        this.result = MsgAdapter.ReadInt();

        this.role_id = MsgAdapter.ReadInt();

        this.role_name = MsgAdapter.ReadStrN(32);

        this.avatar = MsgAdapter.ReadChar();

        this.sex = MsgAdapter.ReadChar();

        this.prof = MsgAdapter.ReadChar();

        this.camp_type = MsgAdapter.ReadChar();

        this.level = MsgAdapter.ReadInt();

        this.create_time = MsgAdapter.ReadUInt();

        UnityLog.Info($"SCCreateRoleAck proto : {this.result}   {this.role_id}    {this.role_name}   {this.avatar} ");
    }
}
