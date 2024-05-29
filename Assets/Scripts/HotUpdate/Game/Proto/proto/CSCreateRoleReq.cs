using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSCreateRoleReq : BaseProtocol
{
    public string plat_name = "";
    public string role_name = "";
    public uint login_time = 0;
    public string key = "";
    public short plat_server_id = 0;
    public sbyte plat_fcm = 0;
    public sbyte avatar = 0;
    public sbyte sex = 0;
    public sbyte prof = 0;
    public sbyte camp_type = 0;
    public string plat_spid = "";

    public override void Init()
    {
        this.msg_type = 7150;

        this.plat_name = "";
        this.role_name = "";
        this.login_time = 0;
        this.key = "";
        this.plat_server_id = 0;
        this.plat_fcm = 0;
        this.avatar = 0;
        this.sex = 0;
        this.prof = 0;
        this.camp_type = 0;
        this.plat_spid = "";
    }

    public override void Encode()
    {
        base.Encode();

        MsgAdapter.WriteBegin(msg_type);
        MsgAdapter.WriteStrN(this.plat_name, 64);
        MsgAdapter.WriteStrN(this.role_name, 32);
        MsgAdapter.WriteUInt(this.login_time);
        MsgAdapter.WriteStrN(this.key, 32);
        MsgAdapter.WriteShort(this.plat_server_id);
        MsgAdapter.WriteChar(this.plat_fcm);
        MsgAdapter.WriteChar(this.avatar);
        MsgAdapter.WriteChar(this.sex);
        MsgAdapter.WriteChar(this.prof);
        MsgAdapter.WriteChar(this.camp_type);
        MsgAdapter.WriteChar(0);
        MsgAdapter.WriteStrN(this.plat_spid, 4);
    }


    public override void Decode()
    {
        //int result = MsgAdapter.ReadInt();
        //int role_id = MsgAdapter.ReadInt();
        //role_name = MsgAdapter.ReadStrN(32);
        //avatar = MsgAdapter.ReadChar();
        ///[ฑํว้]x = MsgAdapter.ReadChar();
        //prof = MsgAdapter.ReadChar();
        //camp_type = MsgAdapter.ReadChar();
        //int level = MsgAdapter.ReadInt();
        //uint create_time = MsgAdapter.ReadUInt();
    }
}
