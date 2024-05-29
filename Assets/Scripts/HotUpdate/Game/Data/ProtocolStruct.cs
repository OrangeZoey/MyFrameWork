using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtocolStruct : Appearance
{
    public static ProtocolStruct instance; 
    public Appearance ReadRoleAppearance()
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
        return this;
    }
}
