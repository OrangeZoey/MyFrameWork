using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 搜索
/// </summary>
public class CSPublicSaleSearch : BaseProtocol
{
    public int item_type = default;           // 指定类型 0为不指定
    public short level = default;             // 指定等级 0为不指定
    public short level_interval = default;    // 等级区间
    public short prof = default;              // 指定职业 0为不指定
    public short color = default;             // 指定颜色 0为不指定
    public short color_interval = default;    // 颜色区间
    public short order = default;             // 指定阶数 0为不指定
    public short order_interval = default;    // 阶数区间
    public short page_item_count = 3;         // 页显示几个物品
    public int req_page = default;            // 请求第几页
    public int total_page = default;          // 总页数 客户端没有总页数的时候 要填成0 如果已经有总页数 填上总页数
    public short sort_type = default;         // 单价排序或总价排序
    public short rank_type = default;         // 排序方式
    public int need_notice = default;         // 是否需要提示
    public int fuzzy_type_count = default;
    public List<Fuzzytype> fuzzy_type_list = new List<Fuzzytype>();

    public override void Init()
    {
        base.Init();
        this.msg_type = 9650;
    }
    public override void Encode()
    {
        base.Encode();
        MsgAdapter.WriteBegin(this.msg_type);

        MsgAdapter.WriteInt(this.item_type);

        MsgAdapter.WriteShort(this.level);

        MsgAdapter.WriteShort(this.level_interval);

        MsgAdapter.WriteShort(this.prof);

        MsgAdapter.WriteShort(this.color);

        MsgAdapter.WriteShort(this.color_interval);

        MsgAdapter.WriteShort(this.order);

        MsgAdapter.WriteShort(this.order_interval);

        MsgAdapter.WriteShort(this.page_item_count);

        MsgAdapter.WriteInt(this.req_page);

        MsgAdapter.WriteInt(this.total_page);

        MsgAdapter.WriteShort(this.sort_type);

        MsgAdapter.WriteShort(this.rank_type);

        MsgAdapter.WriteInt(this.need_notice);

        MsgAdapter.WriteInt(this.fuzzy_type_count);

        for (int i = 0; i < this.fuzzy_type_count; i++)
        {
            Fuzzytype fuzzy_type = fuzzy_type_list[i];

            MsgAdapter.WriteInt(fuzzy_type.item_sale_type);
            MsgAdapter.WriteInt(fuzzy_type.item_count);

            List<int> item_id_list = fuzzy_type.item_id_list;
            for (int j = 0; j < item_id_list.Count; j++)
            {
                MsgAdapter.WriteInt(item_id_list[j]);
            }
        }
    }
    public override void Decode()
    {

    }
}

public class Fuzzytype
{
    public int item_sale_type;
    public int item_count;
    public List<int> item_id_list = new List<int>();
}

public class SearchConfig
{
    public int item_type = default;           // 指定类型 0为不指定        
    public short prof = default;              // 指定职业 0为不指定
    public short color = default;             // 指定颜色 0为不指定    
    public short order = default;             // 指定阶数 0为不指定
    public int req_page = 1;            // 请求第几页
    public int total_page = default;          // 总页数 客户端没有总页数的时候 要填成0 如果已经有总页数 填上总页数
    public short sort_type = default;         // 单价排序或总价排序
    public short rank_type = default;         // 排序方式
    public int need_notice = default;         // 是否需要提示
    public int fuzzy_type_count = default;
    public List<Fuzzytype> fuzzy_type_list = new List<Fuzzytype>();
}
