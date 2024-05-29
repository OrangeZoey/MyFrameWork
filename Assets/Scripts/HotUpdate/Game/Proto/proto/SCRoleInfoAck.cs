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
        this.attr_t.origin_merge_server_id = MsgAdapter.ReadInt();//�����ԭ��ID
        this.attr_t.sex = MsgAdapter.ReadChar();
        this.attr_t.prof = MsgAdapter.ReadChar();
        this.attr_t.camp = MsgAdapter.ReadChar();
        this.attr_t.authority_type = MsgAdapter.ReadChar();//������ͣ�����
        this.attr_t.level = MsgAdapter.ReadShort();
        this.attr_t.energy = MsgAdapter.ReadShort();

        this.attr_t.hp = MsgAdapter.ReadInt();
        this.attr_t.base_max_hp = MsgAdapter.ReadInt();
        this.attr_t.mp = MsgAdapter.ReadInt();
        this.attr_t.base_max_mp = MsgAdapter.ReadInt();

        this.attr_t.base_gongji = MsgAdapter.ReadInt();//��������
        this.attr_t.base_fangyu = MsgAdapter.ReadInt();//��������
        this.attr_t.base_mingzhong = MsgAdapter.ReadInt();// ��������
        this.attr_t.base_shanbi = MsgAdapter.ReadInt();//��������
        this.attr_t.base_baoji = MsgAdapter.ReadInt();//��������
        this.attr_t.base_jianren = MsgAdapter.ReadInt();// ��������

        this.attr_t.base_ignore_fangyu = MsgAdapter.ReadInt();// �������ӷ���
        this.attr_t.base_hurt_increase = MsgAdapter.ReadInt();// �����˺�׷��
        this.attr_t.base_hurt_reduce = MsgAdapter.ReadInt();// �����˺�����
        this.attr_t.base_ice_master = MsgAdapter.ReadInt();// ��������ͨ
        this.attr_t.base_fire_master = MsgAdapter.ReadInt();// ������ͨ
        this.attr_t.base_thunder_master = MsgAdapter.ReadInt();// �����׾�ͨ
        this.attr_t.base_poison_master = MsgAdapter.ReadInt();// ��������ͨ

        this.attr_t.base_move_speed = MsgAdapter.ReadInt();// �����ƶ��ٶ�
        this.attr_t.base_fujia_shanghai = MsgAdapter.ReadInt();// �����˺�
        this.attr_t.base_dikang_shanghai = MsgAdapter.ReadInt();// �ֿ��˺�
        this.attr_t.base_constant_zengshang = MsgAdapter.ReadInt();// �̶�����
        this.attr_t.base_constant_mianshang = MsgAdapter.ReadInt();// �̶�����

        this.attr_t.base_per_mingzhong = MsgAdapter.ReadInt();// ������
        this.attr_t.base_per_shanbi = MsgAdapter.ReadInt();// ������
        this.attr_t.base_per_jingzhun = MsgAdapter.ReadInt();// ��׼��
        this.attr_t.base_per_baoji = MsgAdapter.ReadInt();// ������
        this.attr_t.base_per_kangbao = MsgAdapter.ReadInt();// ������
        this.attr_t.base_per_pofang = MsgAdapter.ReadInt();// �˺�������
        this.attr_t.base_per_mianshang = MsgAdapter.ReadInt();// �˺�������
        this.attr_t.base_per_pvp_hurt_increase = MsgAdapter.ReadInt();// pvp�˺�������
        this.attr_t.base_per_pvp_hurt_reduce = MsgAdapter.ReadInt();// pvp���˼�����
        this.attr_t.base_per_xixue = MsgAdapter.ReadInt();// ��Ѫ��
        this.attr_t.base_per_stun = MsgAdapter.ReadInt();// ������

        this.attr_t.exp = MsgAdapter.ReadLL();// ����
        this.attr_t.max_exp = MsgAdapter.ReadLL();// �����
        this.attr_t.attack_mode = MsgAdapter.ReadChar();// ����ģʽ
        this.attr_t.name_color = MsgAdapter.ReadChar();// ������ɫ
        this.attr_t.move_mode = MsgAdapter.ReadChar();// �˶�ģʽ
        this.attr_t.move_mode_param = MsgAdapter.ReadChar();// �˶�ģʽ����
        this.attr_t.xiannv_huanhua_id = MsgAdapter.ReadShort();// ��Ů�û�id
        this.attr_t.jump_remain_times = MsgAdapter.ReadShort();// ��Ծʣ�����
        this.attr_t.jump_last_recover_time = MsgAdapter.ReadUInt();// ���ָ���Ծʱ��


        this.attr_t.capability = MsgAdapter.ReadInt();// ս����

        this.attr_t.buff_mark_low = MsgAdapter.ReadInt();// buffЧ����ǵ�λ
        this.attr_t.buff_mark_high = MsgAdapter.ReadInt();// buffЧ����Ǹ�λ

        this.attr_t.evil = MsgAdapter.ReadInt();// ���ֵ
        this.attr_t.xianhun = MsgAdapter.ReadInt();// �ɻ�
        this.attr_t.yuanli = MsgAdapter.ReadInt();// Ԫ��
        this.attr_t.nv_wa_shi = MsgAdapter.ReadInt();// Ů�ʯ
        this.attr_t.lingjing = MsgAdapter.ReadInt();// �龧
        this.attr_t.chengjiu = MsgAdapter.ReadInt();// �ɾ�
        this.attr_t.hunli = MsgAdapter.ReadInt();// �۳�

        this.attr_t.guild_id = MsgAdapter.ReadInt();// ����ID
        this.attr_t.guild_name = MsgAdapter.ReadStrN(32);// ��������
        this.attr_t.last_leave_guild_time = MsgAdapter.ReadUInt();// ����뿪����ʱ��
        this.attr_t.guild_avatar_key_big = MsgAdapter.ReadUInt();// ���Ŵ�ͷ��
        this.attr_t.guild_avatar_key_small = MsgAdapter.ReadUInt();// ����Сͷ��
        this.attr_t.guild_post = MsgAdapter.ReadChar();// ����ְλ
        this.attr_t.is_team_leader = MsgAdapter.ReadChar();// ��Ӷӳ���־
        this.attr_t.mount_appeid = MsgAdapter.ReadShort();// �������

        this.attr_t.husong_color = MsgAdapter.ReadChar();// ����������ɫ
        this.attr_t.is_change_avatar = MsgAdapter.ReadChar();// �Ƿ񻻹�ͷ��
        this.attr_t.husong_taskid = MsgAdapter.ReadUShort();// ��������ID

        this.attr_t.nuqi = MsgAdapter.ReadInt();// ŭ��
        this.attr_t.honour = MsgAdapter.ReadInt();// ����

        this.attr_t.guild_gongxian = MsgAdapter.ReadInt();// ����
        this.attr_t.guild_total_gongxian = MsgAdapter.ReadInt();// �ܹ���

        this.attr_t.max_hp = MsgAdapter.ReadInt();// ս�����Ѫ��
        this.attr_t.max_mp = MsgAdapter.ReadInt();// ս�������
        this.attr_t.gong_ji = MsgAdapter.ReadInt();// ս������
        this.attr_t.fang_yu = MsgAdapter.ReadInt();// ս������
        this.attr_t.ming_zhong = MsgAdapter.ReadInt();// ս������
        this.attr_t.shan_bi = MsgAdapter.ReadInt();// ս������
        this.attr_t.bao_ji = MsgAdapter.ReadInt();// ս������
        this.attr_t.jian_ren = MsgAdapter.ReadInt();// ս������
        this.attr_t.move_speed = MsgAdapter.ReadInt();// ս���ƶ��ٶ�
        this.attr_t.fujia_shanghai = MsgAdapter.ReadInt();// �����˺�
        this.attr_t.dikang_shanghai = MsgAdapter.ReadInt();// �ֿ��˺�
        this.attr_t.ignore_fangyu = MsgAdapter.ReadInt();// ���ӷ���
        this.attr_t.hurt_increase = MsgAdapter.ReadInt();// �˺�׷��
        this.attr_t.hurt_reduce = MsgAdapter.ReadInt();// �˺�����
        this.attr_t.ice_master = MsgAdapter.ReadInt();// ����ͨ
        this.attr_t.fire_master = MsgAdapter.ReadInt();// ��ͨ
        this.attr_t.thunder_master = MsgAdapter.ReadInt();// �׾�ͨ
        this.attr_t.poison_master = MsgAdapter.ReadInt();// ����ͨ
        this.attr_t.constant_zengshang = MsgAdapter.ReadInt();// �̶�����
        this.attr_t.constant_mianshang = MsgAdapter.ReadInt();// �̶�����

        this.attr_t.per_mingzhong = MsgAdapter.ReadInt();// ������
        this.attr_t.per_shanbi = MsgAdapter.ReadInt();// ������
        this.attr_t.per_jingzhun = MsgAdapter.ReadInt();// ��׼���Ƽ��ʣ�
        this.attr_t.per_baoji = MsgAdapter.ReadInt();// ������
        this.attr_t.per_baoji_hurt = MsgAdapter.ReadInt();// �����˺���
        this.attr_t.per_kangbao = MsgAdapter.ReadInt();// �����ʣ�������
        this.attr_t.per_pofang = MsgAdapter.ReadInt();// �˺�������
        this.attr_t.per_mianshang = MsgAdapter.ReadInt();// �˺�������
        this.attr_t.per_pvp_hurt_increase = MsgAdapter.ReadInt();// pvp�˺�������
        this.attr_t.per_pvp_hurt_reduce = MsgAdapter.ReadInt();// pvp���˼�����
        this.attr_t.per_xixue = MsgAdapter.ReadInt();// ��Ѫ��
        this.attr_t.per_stun = MsgAdapter.ReadInt();// ������

        this.attr_t.appearance =new Appearance();// ���


        this.attr_t.used_sprite_grade = MsgAdapter.ReadChar();// ��Ů�񽻵ȼ�
        this.attr_t.used_sprite_quality = MsgAdapter.ReadChar();// ��ŮƷ�ʵȼ�
        this.attr_t.flyup_use_image = MsgAdapter.ReadShort();
        this.attr_t.chengjiu_title_level = MsgAdapter.ReadShort();// ��Ů����ȼ�
        this.attr_t.used_sprite_id = MsgAdapter.ReadUShort();// ��Ůid
        this.attr_t.sprite_name = MsgAdapter.ReadStrN(32);// ��Ů����
        this.attr_t.shengwang = MsgAdapter.ReadInt();// ����
        this.attr_t.avatar_key_big = MsgAdapter.ReadUInt();// ��ͷ��
        this.attr_t.avatar_key_small = MsgAdapter.ReadUInt();// Сͷ��
        this.attr_t.lover_uid = MsgAdapter.ReadInt();// ����uid
        this.attr_t.lover_name = MsgAdapter.ReadStrN(32);// ��������
        this.attr_t.last_marry_time = MsgAdapter.ReadUInt();// �ϴν��ʱ��
        this.attr_t.use_sprite_halo_img = MsgAdapter.ReadShort();// ��Ů�⻷����
        this.attr_t.used_sprite_jie = MsgAdapter.ReadShort();// ʹ����Ů�Ľ���
        this.attr_t.xianjie_level = MsgAdapter.ReadShort();
        this.attr_t.day_revival_times = MsgAdapter.ReadUShort();// ���ո������
        this.attr_t.cross_honor = MsgAdapter.ReadInt();// �������
        this.attr_t.plat_type = MsgAdapter.ReadInt();// ƽ̨����
        this.attr_t.jinghua_husong_status = MsgAdapter.ReadChar();// ��������״̬
        this.attr_t.use_sprite_imageid = MsgAdapter.ReadChar();// �����������
        this.attr_t.user_pet_special_img = MsgAdapter.ReadShort();// ����û�
        this.attr_t.gongxun = MsgAdapter.ReadInt();// ��ѫ

        this.attr_t.pet_id = MsgAdapter.ReadInt();// ����ID
        this.attr_t.pet_level = MsgAdapter.ReadShort();// ����ȼ�
        this.attr_t.pet_grade = MsgAdapter.ReadShort();// ����׼�
        this.attr_t.pet_name = MsgAdapter.ReadStrN(32);// ��������
        this.attr_t.user_lq_special_img = MsgAdapter.ReadShort();// ������������
        this.attr_t.use_xiannv_halo_img = MsgAdapter.ReadShort();// ����⻷����

        this.attr_t.multi_mount_res_id = MsgAdapter.ReadShort();// ˫������ID
        this.attr_t.multi_mount_is_owner = MsgAdapter.ReadShort();// �Ƿ���˫�����������
        this.attr_t.multi_mount_other_uid = MsgAdapter.ReadInt();// һ����˵����obj_id

        this.attr_t.little_pet_name = MsgAdapter.ReadStrN(32);// С��������
        this.attr_t.little_pet_using_id = MsgAdapter.ReadShort();// С��������id
        this.attr_t.fight_mount_appeid = MsgAdapter.ReadShort();// ս���������


        this.attr_t.total_strengthen_level = MsgAdapter.ReadShort();// ȫ����ǿ���ȼ�
        this.attr_t.beauty_used_seq = MsgAdapter.ReadChar();// ����seq
        this.attr_t.beauty_is_active_shenwu = MsgAdapter.ReadChar();// �����Ƿ��Ѽ�������


        this.attr_t.camp_post = MsgAdapter.ReadChar();// ��Ӫ(����)��ְ
        this.attr_t.is_neijian = MsgAdapter.ReadChar();// �Ƿ��Ǳ������ڼ�
        this.attr_t.citan_color = MsgAdapter.ReadChar();// ��̽�����õ�����ɫ
        this.attr_t.banzhuan_color = MsgAdapter.ReadChar();// ��ש�����õ�����ɫ



        this.attr_t.beauty_used_huanhua_seq = MsgAdapter.ReadChar();// ���˻û�seq
        this.attr_t.wing_used_huanhua_seq = MsgAdapter.ReadChar();// ����û�seq
        this.attr_t.mount_used_huanhua_seq = MsgAdapter.ReadChar();// ����û�seq
        this.attr_t.server_group = MsgAdapter.ReadChar();// ��������(���� / ����)
        // this.attr_t.wuqi_color       = MsgAdapter.ReadInt();// ������ɫ

        this.attr_t.hold_beauty_npcid = MsgAdapter.ReadUShort();// �����˵�NPC ID
        this.attr_t.jingling_guanghuan_img_id = this.attr_t.appearance.jingling_guanghuan_imageid;// ���˹⻷
        this.attr_t.junxian_level = MsgAdapter.ReadShort();// ���εȼ�
        this.attr_t.baojia_speical_image_id = MsgAdapter.ReadShort();// ����������װ
        MsgAdapter.ReadShort();
        this.attr_t.touxian_level = MsgAdapter.ReadInt();// ͷ�εȼ�
        this.attr_t.uuid = MsgAdapter.ReadLL();// ��ɫuuid
        this.attr_t.origin_role_id = MsgAdapter.ReadInt();// ԭ��uid
        this.attr_t.baby_id = MsgAdapter.ReadShort();// չʾ�ı���ID
        MsgAdapter.ReadShort();
        this.attr_t.origin_open_day = MsgAdapter.ReadInt();

    }
}
