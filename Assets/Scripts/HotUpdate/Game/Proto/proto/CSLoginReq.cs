public class CSLoginReq : BaseProtocol
{
    private bool is_game_world_protocol;
    public bool Is_game_world_protocol { get => is_game_world_protocol; }

    public int rand_1;
    public uint login_time;
    public string key = "";
    public string plat_name = "";
    public int rand_2 = 0;
    public short plat_fcm = 0;
    public short plat_server_id = 0;


    public override void Init()
    {
        this.msg_type = 7056;
        is_game_world_protocol = false;


        rand_1 = default;
        login_time = default;
        key = default;
        plat_name = default;
        rand_2 = default;
        plat_fcm = default;
        plat_server_id = default;
        
    }
  


    public override void Decode()
    {

    }

    public override void Encode()
    {
        base.Encode();

        MsgAdapter.WriteBegin(this.msg_type);

        MsgAdapter.WriteInt(this.rand_1);

        MsgAdapter.WriteUInt(this.login_time);

        MsgAdapter.WriteStrN(this.key, 32);

        MsgAdapter.WriteStrN(this.plat_name, 64);

        MsgAdapter.WriteInt(this.rand_2);

        MsgAdapter.WriteShort(this.plat_fcm);

        MsgAdapter.WriteShort(this.plat_server_id);
    }


}

