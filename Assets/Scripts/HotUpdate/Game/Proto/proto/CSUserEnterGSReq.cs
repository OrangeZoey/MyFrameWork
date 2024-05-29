public class CSUserEnterGSReq : BaseProtocol
{
    public int scene_id = 0;
    public int scene_key = 0;
    public int last_scene_id = 0;
    public int role_id = 0;
    public string role_name = "";
    public int time = 0;
    public sbyte is_login = 0;
    public short server_id = 0;
    public string key = "";
    public string plat_name = "";
    public int is_micro_pc = 0;
    public string plat_spid = "";
    public override void Init()
    {
        base.Init();
        this.msg_type = 1050;

        this.scene_id = 0;
        this.scene_key = 0;
        this.last_scene_id = 0;
        this.role_id = 0;
        this.role_name = "";
        this.time = 0;
        this.is_login = 0;
        this.server_id = 0;
        this.key = "";
        this.plat_name = "";
        this.is_micro_pc = 0;
        this.plat_spid = "";

    }
    public override void Decode()
    {

    }

    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(this.msg_type);
        MsgAdapter.WriteInt(this.scene_id);
        MsgAdapter.WriteInt(this.scene_key);
        MsgAdapter.WriteInt(this.last_scene_id);
        MsgAdapter.WriteInt(this.role_id);
        MsgAdapter.WriteStrN(this.role_name, 32);
        MsgAdapter.WriteInt(this.time);
        MsgAdapter.WriteChar(this.is_login);
        MsgAdapter.WriteChar(0);
        MsgAdapter.WriteShort(this.server_id);
        MsgAdapter.WriteStrN(this.key, 32);
        MsgAdapter.WriteStrN(this.plat_name, 64);
        MsgAdapter.WriteInt(this.is_micro_pc);
        MsgAdapter.WriteStrN(this.plat_spid, 4);

    }
}

