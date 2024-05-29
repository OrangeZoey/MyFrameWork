using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtocolStruct : Appearance
{
    public static ProtocolStruct instance; 
    public Appearance ReadRoleAppearance()
    {

        this.wuqi_use_type = MsgAdapter.ReadShort();// ���ʹ���������ͣ������ͻ��ۣ�
        this.body_use_type = MsgAdapter.ReadShort();// ���ʹ���·�����
        this.wuqi_id = MsgAdapter.ReadUShort();
        this.fashion_wuqi = MsgAdapter.ReadChar();
        this.fashion_body = MsgAdapter.ReadChar();
        this.mount_used_imageid = MsgAdapter.ReadShort();
        this.wing_used_imageid = MsgAdapter.ReadShort();
        this.wing_used_imageid = MsgAdapter.ReadShort();
        this.halo_used_imageid = MsgAdapter.ReadShort();
        this.shengong_used_imageid = MsgAdapter.ReadShort();
        this.shenyi_used_imageid = MsgAdapter.ReadShort();
        this.xiannvshouhu_imageid = MsgAdapter.ReadShort();
        this.jingling_guanghuan_imageid = MsgAdapter.ReadShort();
        this.jingling_fazhen_imageid = MsgAdapter.ReadShort();
        this.fight_mount_used_imageid = MsgAdapter.ReadShort();
        this.zhibao_used_imageid = MsgAdapter.ReadShort();
        this.shengbing_image_id = MsgAdapter.ReadChar();// �������id
        this.shengbing_texiao_id = MsgAdapter.ReadChar();// �����Чid
        this.baojia_image_id = MsgAdapter.ReadChar();// ��������id
        this.baojia_texiao_id = MsgAdapter.ReadChar();// ������Чid
        this.fazhen_image_id = MsgAdapter.ReadShort();// ��������id
        this.ugs_head_wear_img_id = MsgAdapter.ReadShort();// ͷ��id
        this.ugs_mask_img_id = MsgAdapter.ReadShort();// ����id
        this.ugs_waist_img_id = MsgAdapter.ReadShort();// ����id
        this.ugs_kirin_arm_img_id = MsgAdapter.ReadShort();// �����id
        this.ugs_bead_img_id = MsgAdapter.ReadShort();// ����id
        this.ugs_fabao_img_id = MsgAdapter.ReadShort();// ����id
        MsgAdapter.ReadShort();
        return this;
    }
}
