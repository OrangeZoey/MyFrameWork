using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nirvana;
using UnityEngine.EventSystems;

public class RoleItem : MonoBehaviour
{
    UINameTable nameTable;
    UIVariableTable table;

    public Role role;
    public void Init(Role role)
    {
        this.role = role;
        table = GetComponent<UIVariableTable>();
        nameTable = GetComponent<UINameTable>();
        if (role.role_name!=null)
        {
            table.FindVariable("Name").SetString(role.role_name);
            table.FindVariable("Level").SetString(role.level.ToString() + "¼¶");
            table.FindVariable("Camp").SetAsset("uis/views/login/images_atlas", "select_guoji_" + role.camp);
            table.FindVariable("HeadImg").SetAsset("uis/views/login/images_atlas", "prof_img_" + role.prof);
        }
        else
        {
            table.FindVariable("IsCreateRole").SetBoolean(true);
        }


    }
}
