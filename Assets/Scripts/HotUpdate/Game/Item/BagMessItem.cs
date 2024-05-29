using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nirvana;
using System;
using UnityEngine.UI;

public class BagMessItem : MonoBehaviour
{
    private UINameTable table;
    private UIVariableTable variableTable;
    public void Init()
    {
        var userInfo = GameManager.ECS.World.GetComponent<PlayerInfoComponent>().userInfo;
        table = GetComponent<UINameTable>();
        variableTable = GetComponent<UIVariableTable>();
        table.Find("Display").GetComponent<RawImage>().enabled = true ;
        table.Find("UICamera").GetComponent<Camera>().enabled = true ;
        table.Find("Image").GetComponent<Image>().enabled = true ;
        variableTable.FindVariable("Portrait").SetAsset("uis/icons/portrait_atlas", userInfo.attr_t.prof+"0");
        variableTable.FindVariable("FightPower").SetInteger(userInfo.attr_t.capability);
        variableTable.FindVariable("Name").SetString(userInfo.role_name);
        variableTable.FindVariable("Level").SetString(userInfo.attr_t.level.ToString());
        variableTable.FindVariable("Guild").SetString(userInfo.attr_t.authority_type.ToString());
        variableTable.FindVariable("CharmValue").SetInteger(userInfo.attr_t.energy);
        variableTable.FindVariable("GongJi").SetInteger(userInfo.attr_t.base_gongji);
        variableTable.FindVariable("HPValue").SetInteger(userInfo.attr_t.max_hp);
        variableTable.FindVariable("FangYu").SetInteger(userInfo.attr_t.base_fangyu);
        variableTable.FindVariable("MingZhong").SetInteger(userInfo.attr_t.base_mingzhong);
        variableTable.FindVariable("PoJia").SetInteger(userInfo.attr_t.base_ignore_fangyu);
        variableTable.FindVariable("ShanBi").SetInteger(userInfo.attr_t.base_shanbi);
        variableTable.FindVariable("ShangHaiJiaCheng").SetInteger(userInfo.attr_t.base_hurt_increase);
        variableTable.FindVariable("BaoJi").SetInteger(userInfo.attr_t.base_baoji);
        variableTable.FindVariable("ShangHaiJianMian").SetInteger(userInfo.attr_t.base_hurt_reduce);
        variableTable.FindVariable("KangBao").SetInteger(userInfo.attr_t.base_jianren);

        variableTable.FindVariable("AtkTong").SetInteger(userInfo.attr_t.base_gongji);
        variableTable.FindVariable("FYTong").SetInteger(userInfo.attr_t.base_fangyu);
        variableTable.FindVariable("QXTong").SetInteger(userInfo.attr_t.max_hp);
        variableTable.FindVariable("BJTong").SetInteger(userInfo.attr_t.base_baoji);
        variableTable.FindVariable("JSTong").SetInteger(userInfo.attr_t.base_hurt_reduce);


    }

 
}
