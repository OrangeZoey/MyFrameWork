using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearance 
{
    public int wuqi_use_type  ;// ���ʹ���������ͣ������ͻ��ۣ�
	public int body_use_type  ;// ���ʹ���·�����
	public int wuqi_id  ;
	public int fashion_wuqi ;
	public int fashion_body ;
	public int mount_used_imageid  ;
	public int wing_used_imageid  ;
	public int halo_used_imageid  ;
	public int shengong_used_imageid  ;
	public int shenyi_used_imageid  ;
	public int xiannvshouhu_imageid  ;
	public int jingling_guanghuan_imageid  ;
	public int jingling_fazhen_imageid  ;
	public int fight_mount_used_imageid  ;
	public int zhibao_used_imageid  ;
	public int shengbing_image_id ;// �������id
	public int shengbing_texiao_id ;// �����Чid
	public int baojia_image_id ;// ��������id
	public int baojia_texiao_id ;// ������Чid
	public int fazhen_image_id  ;// ��������id
	public int ugs_head_wear_img_id  ;// ͷ��id
	public int ugs_mask_img_id  ;// ����id
	public int ugs_waist_img_id  ;// ����id
	public int ugs_kirin_arm_img_id  ;// �����id
	public int ugs_bead_img_id  ;// ����id
	public int ugs_fabao_img_id  ;// ����id

	public Appearance()
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
    }
}
