using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于扩展IList<T>接口的功能
/// </summary>
public static class ListExt 
{
    /// <summary>
    /// 无GC版本的AddRange
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="collection"></param>
    public static void AddRangeNonAlloc<T>(this IList<T> list, IList<T> collection)
    {
        //将一个集合（collection）的所有元素添加到另一个列表（list）中，而不需要分配额外的内存。
        
        //为空  直接结束
        if (collection == null)
            return;

        //这是通过遍历 collection 并逐个将元素添加到 list 中来实现
        for (int i = 0; i < collection.Count; i++)
        {
            list.Add(collection[i]);
        }
    }

    /// <summary>
    /// 有序插入,将一个元素插入到已经排序好的list中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="element"></param>
    public static int OrderedInsert<T>(this IList<T> list, T element) where T : IComparable<T>
    {
        //使用了二分查找算法来快速找到插入位置。
        //这个方法要求列表中的元素实现了 IComparable<T> 接口，以便能够进行比较。
        //如果列表为空，元素会被添加到列表的开头。方法返回插入元素的索引位置

        if (list.Count == 0)
        {
            list.Add(element);
            return 0;
        }

        int start = 0;
        int end = list.Count - 1;
        int index = list.Count;
        while (start < end)
        {
            index = (start + end) / 2;
            int curr = list[index].CompareTo(element);
            int next = list[index + 1].CompareTo(element);
            if (curr > 0)
            {
                end = index - 1;
                continue;
            }
            if (next <= 0)
            {
                start = index + 1;
                continue;
            }

            //找到位置了
            list.Insert(index + 1, element);
            return index + 1;
        }

        if (start == end)
        {
            index = list[start].CompareTo(element) <= 0 ? start + 1 : start;
        }

        list.Insert(index, element);
        return index;
    }

    public static T Dequeue<T>(this IList<T> list)
    {
        //从列表的开头移除并返回一个元素。
        //它首先获取列表的第一个元素，然后移除它，并返回该元素。
        //这类似于队列（Queue）的出队操作，但直接在列表上实现

        T element = list[0];
        list.RemoveAt(0);
        return element;
    }
}
