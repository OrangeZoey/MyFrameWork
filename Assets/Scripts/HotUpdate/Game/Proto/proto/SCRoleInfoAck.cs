using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCRoleInfoAck : BaseProtocol
{
    public Attr_t attr_t;
    public override void Init()
    {
        base.Init();
        this.msg_type = 1400;
    }
    public override void Decode()
    {
        this.attr_t = new Attr_t();
        this.attr_t.origin_merge_server_id = MsgAdapter.ReadInt();//跨服后原服ID
        this.attr_t.sex = MsgAdapter.ReadChar();
        this.attr_t.prof = MsgAdapter.ReadChar();
        this.attr_t.camp = MsgAdapter.ReadChar();
        this.attr_t.authority_type = MsgAdapter.ReadChar();//身份类型，待定
        this.attr_t.level = MsgAdapter.ReadShort();
        this.attr_t.energy = MsgAdapter.ReadShort();

        this.attr_t.hp = MsgAdapter.ReadInt();
        this.attr_t.base_max_hp = MsgAdapter.ReadInt();
        this.attr_t.mp = MsgAdapter.ReadInt();
        this.attr_t.base_max_mp = MsgAdapter.ReadInt();

        this.attr_t.base_gongji = MsgAdapter.ReadInt();//基础攻击
        this.attr_t.base_fangyu = MsgAdapter.ReadInt();//基础防御
        this.attr_t.base_mingzhong = MsgAdapter.ReadInt();// 基础命中
        this.attr_t.base_shanbi = MsgAdapter.ReadInt();//基础闪避
        this.attr_t.base_baoji = MsgAdapter.ReadInt();//基础暴击
        this.attr_t.base_jianren = MsgAdapter.ReadInt();// 基础坚韧

        this.attr_t.base_ignore_fangyu = MsgAdapter.ReadInt();// 基础无视防御
        this.attr_t.base_hurt_increase = MsgAdapter.ReadInt();// 基础伤害追加
        this.attr_t.base_hurt_reduce = MsgAdapter.ReadInt();// 基础伤害减免
        this.attr_t.base_ice_master = MsgAdapter.ReadInt();// 基础冰精通
        this.attr_t.base_fire_master = MsgAdapter.ReadInt();// 基础火精通
        this.attr_t.base_thunder_master = MsgAdapter.ReadInt();// 基础雷精通
        this.attr_t.base_poison_master = MsgAdapter.ReadInt();// 基础毒精通

        this.attr_t.base_move_speed = MsgAdapter.ReadInt();// 基础移动速度
        this.attr_t.base_fujia_shanghai = MsgAdapter.ReadInt();// 附加伤害
        this.attr_t.base_dikang_shanghai = MsgAdapter.ReadInt();// 抵抗伤害
        this.attr_t.base_constant_zengshang = MsgAdapter.ReadInt();// 固定增伤
        this.attr_t.base_constant_mianshang = MsgAdapter.ReadInt();// 固定免伤

        this.attr_t.base_per_mingzhong = MsgAdapter.ReadInt();// 命中率
        this.attr_t.base_per_shanbi = MsgAdapter.ReadInt();// 闪避率
        this.attr_t.base_per_jingzhun = MsgAdapter.ReadInt();// 精准率
        this.attr_t.base_per_baoji = MsgAdapter.ReadInt();// 暴击率
        this.attr_t.base_per_kangbao = MsgAdapter.ReadInt();// 抗暴率
        this.attr_t.base_per_pofang = MsgAdapter.ReadInt();// 伤害增加率
        this.attr_t.base_per_mianshang = MsgAdapter.ReadInt();// 伤害减少率
        this.attr_t.base_per_pvp_hurt_increase = MsgAdapter.ReadInt();// pvp伤害增加率
        this.attr_t.base_per_pvp_hurt_reduce = MsgAdapter.ReadInt();// pvp受伤减免率
        this.attr_t.base_per_xixue = MsgAdapter.ReadInt();// 吸血率
        this.attr_t.base_per_stun = MsgAdapter.ReadInt();// 击晕率

        this.attr_t.exp = MsgAdapter.ReadLL();// 经验
        this.attr_t.max_exp = MsgAdapter.ReadLL();// 最大经验
        this.attr_t.attack_mode = MsgAdapter.ReadChar();// 攻击模式
        this.attr_t.name_color = MsgAdapter.ReadChar();// 名字颜色
        this.attr_t.move_mode = MsgAdapter.ReadChar();// 运动模式
        this.attr_t.move_mode_param = MsgAdapter.ReadChar();// 运动模式参数
        this.attr_t.xiannv_huanhua_id = MsgAdapter.ReadShort();// 仙女幻化id
        this.attr_t.jump_remain_times = MsgAdapter.ReadShort();// 跳跃剩余次数
        this.attr_t.jump_last_recover_time = MsgAdapter.ReadUInt();// 最后恢复跳跃时间


        this.attr_t.capability = MsgAdapter.ReadInt();// 战斗力

        this.attr_t.buff_mark_low = MsgAdapter.ReadInt();// buff效果标记低位
        this.attr_t.buff_mark_high = MsgAdapter.ReadInt();// buff效果标记高位

        this.attr_t.evil = MsgAdapter.ReadInt();// 罪恶值
        this.attr_t.xianhun = MsgAdapter.ReadInt();// 仙魂
        this.attr_t.yuanli = MsgAdapter.ReadInt();// 元力
        this.attr_t.nv_wa_shi = MsgAdapter.ReadInt();// 女娲石
        this.attr_t.lingjing = MsgAdapter.ReadInt();// 灵晶
        this.attr_t.chengjiu = MsgAdapter.ReadInt();// 成就
        this.attr_t.hunli = MsgAdapter.ReadInt();// 粉尘

        this.attr_t.guild_id = MsgAdapter.ReadInt();// 军团ID
        this.attr_t.guild_name = MsgAdapter.ReadStrN(32);// 军团名字
        this.attr_t.last_leave_guild_time = MsgAdapter.ReadUInt();// 最后离开军团时间
        this.attr_t.guild_avatar_key_big = MsgAdapter.ReadUInt();// 军团大头像
        this.attr_t.guild_avatar_key_small = MsgAdapter.ReadUInt();// 军团小头像
        this.attr_t.guild_post = MsgAdapter.ReadChar();// 军团职位
        this.attr_t.is_team_leader = MsgAdapter.ReadChar();// 组队队长标志
        this.attr_t.mount_appeid = MsgAdapter.ReadShort();// 坐骑外观

        this.attr_t.husong_color = MsgAdapter.ReadChar();// 护送任务颜色
        this.attr_t.is_change_avatar = MsgAdapter.ReadChar();// 是否换过头像
        this.attr_t.husong_taskid = MsgAdapter.ReadUShort();// 护送任务ID

        this.attr_t.nuqi = MsgAdapter.ReadInt();// 怒气
        this.attr_t.honour = MsgAdapter.ReadInt();// 荣誉

        this.attr_t.guild_gongxian = MsgAdapter.ReadInt();// 贡献
        this.attr_t.guild_total_gongxian = MsgAdapter.ReadInt();// 总贡献

        this.attr_t.max_hp = MsgAdapter.ReadInt();// 战斗最大血量
        this.attr_t.max_mp = MsgAdapter.ReadInt();// 战斗最大法力
        this.attr_t.gong_ji = MsgAdapter.ReadInt();// 战斗攻击
        this.attr_t.fang_yu = MsgAdapter.ReadInt();// 战斗防御
        this.attr_t.ming_zhong = MsgAdapter.ReadInt();// 战斗命中
        this.attr_t.shan_bi = MsgAdapter.ReadInt();// 战斗闪避
        this.attr_t.bao_ji = MsgAdapter.ReadInt();// 战斗暴击
        this.attr_t.jian_ren = MsgAdapter.ReadInt();// 战斗坚韧
        this.attr_t.move_speed = MsgAdapter.ReadInt();// 战斗移动速度
        this.attr_t.fujia_shanghai = MsgAdapter.ReadInt();// 附加伤害
        this.attr_t.dikang_shanghai = MsgAdapter.ReadInt();// 抵抗伤害
        this.attr_t.ignore_fangyu = MsgAdapter.ReadInt();// 无视防御
        this.attr_t.hurt_increase = MsgAdapter.ReadInt();// 伤害追加
        this.attr_t.hurt_reduce = MsgAdapter.ReadInt();// 伤害减免
        this.attr_t.ice_master = MsgAdapter.ReadInt();// 冰精通
        this.attr_t.fire_master = MsgAdapter.ReadInt();// 火精通
        this.attr_t.thunder_master = MsgAdapter.ReadInt();// 雷精通
        this.attr_t.poison_master = MsgAdapter.ReadInt();// 毒精通
        this.attr_t.constant_zengshang = MsgAdapter.ReadInt();// 固定增伤
        this.attr_t.constant_mianshang = MsgAdapter.ReadInt();// 固定免伤

        this.attr_t.per_mingzhong = MsgAdapter.ReadInt();// 命中率
        this.attr_t.per_shanbi = MsgAdapter.ReadInt();// 闪避率
        this.attr_t.per_jingzhun = MsgAdapter.ReadInt();// 精准（破甲率）
        this.attr_t.per_baoji = MsgAdapter.ReadInt();// 暴击率
        this.attr_t.per_baoji_hurt = MsgAdapter.ReadInt();// 暴击伤害率
        this.attr_t.per_kangbao = MsgAdapter.ReadInt();// 抗暴率（废弃）
        this.attr_t.per_pofang = MsgAdapter.ReadInt();// 伤害增加率
        this.attr_t.per_mianshang = MsgAdapter.ReadInt();// 伤害减少率
        this.attr_t.per_pvp_hurt_increase = MsgAdapter.ReadInt();// pvp伤害增加率
        this.attr_t.per_pvp_hurt_reduce = MsgAdapter.ReadInt();// pvp受伤减免率
        this.attr_t.per_xixue = MsgAdapter.ReadInt();// 吸血率
        this.attr_t.per_stun = MsgAdapter.ReadInt();// 击晕率

        this.attr_t.appearance =new Appearance();// 外观


        this.attr_t.used_sprite_grade = MsgAdapter.ReadChar();// 仙女神交等级
        this.attr_t.used_sprite_quality = MsgAdapter.ReadChar();// 仙女品质等级
        this.attr_t.flyup_use_image = MsgAdapter.ReadShort();
        this.attr_t.chengjiu_title_level = MsgAdapter.ReadShort();// 仙女缠绵等级
        this.attr_t.used_sprite_id = MsgAdapter.ReadUShort();// 仙女id
        this.attr_t.sprite_name = MsgAdapter.ReadStrN(32);// 仙女名字
        this.attr_t.shengwang = MsgAdapter.ReadInt();// 声望
        this.attr_t.avatar_key_big = MsgAdapter.ReadUInt();// 大头像
        this.attr_t.avatar_key_small = MsgAdapter.ReadUInt();// 小头像
        this.attr_t.lover_uid = MsgAdapter.ReadInt();// 伴侣uid
        this.attr_t.lover_name = MsgAdapter.ReadStrN(32);// 伴侣名字
        this.attr_t.last_marry_time = MsgAdapter.ReadUInt();// 上次结婚时间
        this.attr_t.use_sprite_halo_img = MsgAdapter.ReadShort();// 仙女光环形象
        this.attr_t.used_sprite_jie = MsgAdapter.ReadShort();// 使用仙女的阶数
        this.attr_t.xianjie_level = MsgAdapter.ReadShort();
        this.attr_t.day_revival_times = MsgAdapter.ReadUShort();// 当日复活次数
        this.attr_t.cross_honor = MsgAdapter.ReadInt();// 跨服荣誉
        this.attr_t.plat_type = MsgAdapter.ReadInt();// 平台类型
        this.attr_t.jinghua_husong_status = MsgAdapter.ReadChar();// 精华护送状态
        this.attr_t.use_sprite_imageid = MsgAdapter.ReadChar();// 精灵飞升形象
        this.attr_t.user_pet_special_img = MsgAdapter.ReadShort();// 精灵幻化
        this.attr_t.gongxun = MsgAdapter.ReadInt();// 功勋

        this.attr_t.pet_id = MsgAdapter.ReadInt();// 宠物ID
        this.attr_t.pet_level = MsgAdapter.ReadShort();// 宠物等级
        this.attr_t.pet_grade = MsgAdapter.ReadShort();// 宠物阶级
        this.attr_t.pet_name = MsgAdapter.ReadStrN(32);// 宠物名字
        this.attr_t.user_lq_special_img = MsgAdapter.ReadShort();// 宠物特殊形象
        this.attr_t.use_xiannv_halo_img = MsgAdapter.ReadShort();// 精灵光环形象

        this.attr_t.multi_mount_res_id = MsgAdapter.ReadShort();// 双人坐骑ID
        this.attr_t.multi_mount_is_owner = MsgAdapter.ReadShort();// 是否是双人坐骑的主人
        this.attr_t.multi_mount_other_uid = MsgAdapter.ReadInt();// 一起骑乘的玩家obj_id

        this.attr_t.little_pet_name = MsgAdapter.ReadStrN(32);// 小宠物名字
        this.attr_t.little_pet_using_id = MsgAdapter.ReadShort();// 小宠物形象id
        this.attr_t.fight_mount_appeid = MsgAdapter.ReadShort();// 战斗坐骑外观


        this.attr_t.total_strengthen_level = MsgAdapter.ReadShort();// 全身总强换等级
        this.attr_t.beauty_used_seq = MsgAdapter.ReadChar();// 美人seq
        this.attr_t.beauty_is_active_shenwu = MsgAdapter.ReadChar();// 美人是否已激活神武


        this.attr_t.camp_post = MsgAdapter.ReadChar();// 阵营(国家)官职
        this.attr_t.is_neijian = MsgAdapter.ReadChar();// 是否是本国的内奸
        this.attr_t.citan_color = MsgAdapter.ReadChar();// 刺探任务拿到的颜色
        this.attr_t.banzhuan_color = MsgAdapter.ReadChar();// 搬砖任务拿到的颜色



        this.attr_t.beauty_used_huanhua_seq = MsgAdapter.ReadChar();// 美人幻化seq
        this.attr_t.wing_used_huanhua_seq = MsgAdapter.ReadChar();// 羽翼幻化seq
        this.attr_t.mount_used_huanhua_seq = MsgAdapter.ReadChar();// 坐骑幻化seq
        this.attr_t.server_group = MsgAdapter.ReadChar();// 服务器组(公族 / 世族)
        // this.attr_t.wuqi_color       = MsgAdapter.ReadInt();// 武器颜色

        this.attr_t.hold_beauty_npcid = MsgAdapter.ReadUShort();// 抱美人的NPC ID
        this.attr_t.jingling_guanghuan_img_id = this.attr_t.appearance.jingling_guanghuan_imageid;// 美人光环
        this.attr_t.junxian_level = MsgAdapter.ReadShort();// 军衔等级
        this.attr_t.baojia_speical_image_id = MsgAdapter.ReadShort();// 穿戴宝甲套装
        MsgAdapter.ReadShort();
        this.attr_t.touxian_level = MsgAdapter.ReadInt();// 头衔等级
        this.attr_t.uuid = MsgAdapter.ReadLL();// 角色uuid
        this.attr_t.origin_role_id = MsgAdapter.ReadInt();// 原服uid
        this.attr_t.baby_id = MsgAdapter.ReadShort();// 展示的宝宝ID
        MsgAdapter.ReadShort();
        this.attr_t.origin_open_day = MsgAdapter.ReadInt();

    }
}
