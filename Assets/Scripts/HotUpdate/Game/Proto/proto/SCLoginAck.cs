using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class SCLoginAck : BaseProtocol
{
    public int role_id;
    /// <summary>
    /// 0  正常     -4游戏世界不存在    其他值，登录失败
    /// </summary>
    public short result;
    public sbyte is_merged_server;
    public int scene_id;
    public int last_scene_id;
    public string key;
    public uint time;
    public string gs_hostname;
    public ushort gs_port;
    public ushort gs_index;
    public uint server_time;

    public override void Init()
    {
        base.Init();
        this.msg_type = 7000;
    }
    public override void Decode()
    {
        this.role_id = MsgAdapter.ReadInt();

        this.result = MsgAdapter.ReadShort();

        MsgAdapter.ReadChar();

        this.is_merged_server = MsgAdapter.ReadChar();

        this.scene_id = MsgAdapter.ReadInt();

        this.last_scene_id = MsgAdapter.ReadInt();

        this.key = MsgAdapter.ReadStrN(32);

        this.time = MsgAdapter.ReadUInt();

        this.gs_hostname = MsgAdapter.ReadStrN(64);

        this.gs_port = MsgAdapter.ReadUShort();

        this.gs_index = MsgAdapter.ReadUShort();

        this.server_time = MsgAdapter.ReadUInt();
    }
}

