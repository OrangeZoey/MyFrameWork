using excellent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ConfigTool
{
    [MenuItem("Tools/导出配置 &#c")]
    public static void ExportConfig()
    {
        //用于存储从程序集中获取的类型
        List<Type> types = new List<Type>();
        //加载程序集并获取其中的类型
        foreach (var type in Assembly.Load("Assembly-CSharp-Editor").GetTypes())
        {
            //命名空间是否为"ConfigDefinition"
            if (type.Namespace == "ConfigDefinition")
            {
                //添加到列表
                types.Add(type);
            }
        }

        //根据传入的信息 导出配置
        Excellent.Go(new ExportInfo()
        {
            //命名空间
            Namespace = "Config",
            //从程序集中获取的类型数组
            ConfigDefinitions = types.ToArray(),
            //分别设置了Excel文件、序列化文件和代码文件的目录路径。
            ExcelDirectory = Application.dataPath + "/../design/config",
            SerializeDirectory = Application.dataPath + "/BundleAssets/Config",
            CodeDirectory = Application.dataPath + "/Scripts/HotUpdate/Config/Code",
            //是否写入Excel文件，这里设置为false
            WriteExcel = false,
            //是否与Unity相关，这里设置为true
            WithUnity = true,
            //BundleOffset = BundleLoader.BundleOffset,
            OnLog = OnLog,
        });
        //刷新Unity的资产数据库
        AssetDatabase.Refresh();
        Debug.Log("导出配置完毕");
    }

    [MenuItem("Tools/更新配置结构 &#v")]
    public static void WriteAndExportConfig()
    {
        //删除目录 true 参数表示如果目录中有子目录或文件，则一并删除
        //为了确保在导出新的配置代码之前，旧的配置代码被彻底清除
        Directory.Delete(Application.dataPath + "/Scripts/HotUpdate/Config/Code", true);

        //查找特定命名空间的类型
        List<Type> types = new List<Type>();
        //加载名为 "Assembly-CSharp-Editor" 的程序集，并遍历其中的所有类型。
        foreach (var type in Assembly.Load("Assembly-CSharp-Editor").GetTypes())
        {
            //如果某个类型的命名空间是 "ConfigDefinition"，则将该类型添加到 types 列表中
            if (type.Namespace == "ConfigDefinition")
            {
                types.Add(type);
            }
        }
        //配置并导出信息:
        Excellent.Go(new ExportInfo()
        {
            Namespace = "Config",
            ConfigDefinitions = types.ToArray(),
            ExcelDirectory = Application.dataPath + "/../../design/config",
            SerializeDirectory = Application.dataPath + "/BundleAssets/Config",
            CodeDirectory = Application.dataPath + "/Scripts/HotUpdate/Config/Code",
            //是否写入Excel文件，这里设置为 true
            WriteExcel = true,
            WithUnity = true,
            //BundleOffset = BundleLoader.BundleOffset,
            OnLog = OnLog,
        });
        //刷新Unity的资产数据库，以确保编辑器能够识别对资产所做的任何更改
        AssetDatabase.Refresh();
        Debug.Log("更新配置结构，并且导出成功");
    }

    private static void OnLog(string message)
    {
        Debug.Log(message);
    }
}