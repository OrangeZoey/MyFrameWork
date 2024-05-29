using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public  class UserInfo
{
    public string plat_name;
    public sbyte plat_fcm;
    public short plat_server_id;
    public string plat_session_key;
    public string role_name;
    public int role_id;
    public int scene_id;
    public int last_scene_id;
    public uint time;
    public int prof;
    public Attr_t attr_t = new Attr_t();

    public PackageData packageData = new PackageData();

    public Item ItemData = new Item();//背包数据 1502

    public PackageInfo info= new PackageInfo(); //背包数据 1500

    public short UserItemIndex;//当前使用的物品

    public Dictionary<int, ItemDataWrapper> equip_list=new Dictionary<int, ItemDataWrapper>();//身上装备列表 1700

    public EquipConfig equipConfig;//当前点击装备栏装备

    public Dictionary<int, List<PackageInfo>> Configstype = new Dictionary<int, List<PackageInfo>>(); //0 全部 1 装备 2 材料 3消耗品 背包数据

}

