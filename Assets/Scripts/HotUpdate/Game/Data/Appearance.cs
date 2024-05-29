using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearance 
{
    public int wuqi_use_type  ;// 外观使用武器类型（解决冲突外观）
	public int body_use_type  ;// 外观使用衣服类型
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
	public int shengbing_image_id ;// 神兵形象id
	public int shengbing_texiao_id ;// 神兵特效id
	public int baojia_image_id ;// 宝甲形象id
	public int baojia_texiao_id ;// 宝甲特效id
	public int fazhen_image_id  ;// 法阵形象id
	public int ugs_head_wear_img_id  ;// 头饰id
	public int ugs_mask_img_id  ;// 面饰id
	public int ugs_waist_img_id  ;// 腰带id
	public int ugs_kirin_arm_img_id  ;// 麒麟臂id
	public int ugs_bead_img_id  ;// 灵珠id
	public int ugs_fabao_img_id  ;// 法宝id

	public Appearance()
    {
        this.wuqi_use_type = MsgAdapter.ReadShort();// 外观使用武器类型（解决冲突外观）
        this.body_use_type = MsgAdapter.ReadShort();// 外观使用衣服类型
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
        this.shengbing_image_id = MsgAdapter.ReadChar();// 神兵形象id
        this.shengbing_texiao_id = MsgAdapter.ReadChar();// 神兵特效id
        this.baojia_image_id = MsgAdapter.ReadChar();// 宝甲形象id
        this.baojia_texiao_id = MsgAdapter.ReadChar();// 宝甲特效id
        this.fazhen_image_id = MsgAdapter.ReadShort();// 法阵形象id
        this.ugs_head_wear_img_id = MsgAdapter.ReadShort();// 头饰id
        this.ugs_mask_img_id = MsgAdapter.ReadShort();// 面饰id
        this.ugs_waist_img_id = MsgAdapter.ReadShort();// 腰带id
        this.ugs_kirin_arm_img_id = MsgAdapter.ReadShort();// 麒麟臂id
        this.ugs_bead_img_id = MsgAdapter.ReadShort();// 灵珠id
        this.ugs_fabao_img_id = MsgAdapter.ReadShort();// 法宝id
        MsgAdapter.ReadShort();
    }
}
