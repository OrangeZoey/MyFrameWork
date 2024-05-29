using System.Collections.Generic;

public class Role
{
    public int role_id;
    public string role_name;
    public sbyte avatar;
    public sbyte sex;
    public sbyte prof;
    public sbyte country;
    public sbyte camp;
    public int level;
    public uint create_time;
    public uint last_login_time;
    public ushort wuqi_id;
    public sbyte shizhuang_wuqi;
    public sbyte shizhuang_body;
    public short wing_used_imageid;
    public short halo_used_imageid;
    public short wuqi_use_type;
    public short body_use_type;
    public short shenbing_img_id;
    public short shenbing_texiao_id;
    public short baojia_img_id;
    public short baojia_texiao_id;
    public short fazhen_used_imageid;
    public RoleAppearance appearance;
}


/// <summary>
/// -- 读取角色外观数据
/// </summary>
public class RoleAppearance
{
    public short wuqi_use_type;            // 外观使用武器类型（解决冲突外观）
    public short body_use_type;            //外观使用衣服类型
    public ushort wuqi_id;
    public sbyte fashion_wuqi;
    public sbyte fashion_body;
    public short mount_used_imageid;
    public short wing_used_imageid;
    public short halo_used_imageid;
    public short shengong_used_imageid;
    public short shenyi_used_imageid;
    public short xiannvshouhu_imageid;
    public short jingling_guanghuan_imageid;
    public short jingling_fazhen_imageid;
    public short fight_mount_used_imageid;
    public short zhibao_used_imageid;
    public sbyte shengbing_image_id;// 神兵形象id
    public sbyte shengbing_texiao_id;// 神兵特效id
    public sbyte baojia_image_id;// 宝甲形象id
    public sbyte baojia_texiao_id;// 宝甲特效id
    public short fazhen_image_id;// 法阵形象id
    public short ugs_head_wear_img_id;// 头饰id
    public short ugs_mask_img_id;// 面饰id
    public short ugs_waist_img_id;// 腰带id
    public short ugs_kirin_arm_img_id;// 麒麟臂id
    public short ugs_bead_img_id;// 灵珠id
    public short ugs_fabao_img_id;// 法宝id

   
}
public class SCRoleListAck : BaseProtocol
{
    public short result;
    public int count;
    public List<Role> role_list;
    public override void Init()
    {
        this.msg_type = 7001;
    }
    public override void Decode()
    {
        MsgAdapter.ReadShort();
        this.result = MsgAdapter.ReadShort();
        this.count = MsgAdapter.ReadInt();

        this.role_list = new List<Role>();
        for (int i = 0; i < this.count; i++)
        {
            Role role = new Role();
            role.role_id = MsgAdapter.ReadInt();
            role.role_name = MsgAdapter.ReadStrN(32);
            role.avatar = MsgAdapter.ReadChar();  ////readChar()---->readSByte()
            role.sex = MsgAdapter.ReadChar();
            role.prof = MsgAdapter.ReadChar();
            role.country = MsgAdapter.ReadChar();
            role.camp = role.country;
            role.level = MsgAdapter.ReadInt();
            role.create_time = MsgAdapter.ReadUInt();
            role.last_login_time = MsgAdapter.ReadUInt();
            role.wuqi_id = MsgAdapter.ReadUShort();
            role.shizhuang_wuqi = MsgAdapter.ReadChar();
            role.shizhuang_body = MsgAdapter.ReadChar();
            role.wing_used_imageid = MsgAdapter.ReadShort();
            role.halo_used_imageid = MsgAdapter.ReadShort();
            role.wuqi_use_type = MsgAdapter.ReadShort();
            role.body_use_type = MsgAdapter.ReadShort();
            role.shenbing_img_id = MsgAdapter.ReadShort();
            role.shenbing_texiao_id = MsgAdapter.ReadShort();
            role.baojia_img_id = MsgAdapter.ReadShort();
            role.baojia_texiao_id = MsgAdapter.ReadShort();
            role.fazhen_used_imageid = MsgAdapter.ReadShort();
            MsgAdapter.ReadShort();
            role.appearance = protocol_struct.ReadRoleAppearance();
            this.role_list.Add(role);
        }

        UnityLog.Info("SCRoleListAck 解析完毕 msgType = 7001");

    }
}

