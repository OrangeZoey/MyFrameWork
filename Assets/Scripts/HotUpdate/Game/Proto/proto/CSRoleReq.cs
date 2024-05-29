public class CSRoleReq : BaseProtocol
{
    public int rand_1 = 0;
    public uint login_time = 0;
    public string key = "";
    public string plat_name = "";
    public short plat_server_id = 0;
    public short plat_fcm;
    public int rand_2 = 0;
    public int role_id = 0;
    public override void Init()
    {
        this.msg_type = 7057;

        this.rand_1 = 0;
        this.login_time = 0;
        this.key = "";
        this.plat_name = "";
        this.plat_server_id = 0;
        this.plat_fcm = 0;
        this.rand_2 = 0;
        this.role_id = 0;
    }

    public override void Encode()
    {
        base.Encode();

        MsgAdapter.WriteBegin(this.msg_type);
        MsgAdapter.WriteInt(this.rand_1);
        MsgAdapter.WriteUInt(this.login_time);
        MsgAdapter.WriteStrN(this.key, 32);
        MsgAdapter.WriteStrN(this.plat_name, 64);
        MsgAdapter.WriteShort(this.plat_server_id);
        MsgAdapter.WriteShort(this.plat_fcm);
        MsgAdapter.WriteInt(this.rand_2);
        MsgAdapter.WriteInt(this.role_id);
    }


    public override void Decode()
    {

    }
}

