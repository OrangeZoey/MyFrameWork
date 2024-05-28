using System;
using System.Collections.Generic;
using TGame.Common;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;

namespace TGame.Editor.Inspector
{
    
    [CustomEditor(typeof(ProcedureModule))]
    public class ProcedureModuleInspector : BaseInspector
    {
        private SerializedProperty proceduresProperty;
        private SerializedProperty defaultProcedureProperty;

        private List<string> allProcedureTypes;

        protected override void OnInspectorEnable()
        {
            base.OnInspectorEnable();
            //serializedObject 是一个SerializedObject实例，它通常代表了一个Unity组件或脚本的序列化状态。通过序列化状态，你可以访问和修改该组件或脚本的所有公开字段和属性。
            // FindProperty 是一个方法，用于在serializedObject中查找一个具有给定名称的属性。
            // 如果找到该属性，它会返回一个SerializedProperty对象，你可以使用这个对象来读取或修改该属性的值。
            //"proceduresNames" 是你想要查找的属性的名称
            proceduresProperty = serializedObject.FindProperty("proceduresNames");
            defaultProcedureProperty = serializedObject.FindProperty("defaultProcedureName");

            UpdateProcedures();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();
            UpdateProcedures();
        }

        private void UpdateProcedures()
        {
            //三个参数：
            //获取继承自 BaseProcedure 类型的所有子类的列表
            //false 不包括抽象子类
            //程序集,如果为null则查找当前程序集
            allProcedureTypes = Utility.Types.GetAllSubclasses(typeof(BaseProcedure), false, Utility.Types.GAME_CSHARP_ASSEMBLY).ConvertAll((Type t) => { return t.FullName; });

            //移除不存在的procedure
            for (int i = proceduresProperty.arraySize - 1; i >= 0; i--)
            {
                //获取当前元素的字符串值（即 procedure 类型的完整名称）
                string procedureTypeName = proceduresProperty.GetArrayElementAtIndex(i).stringValue;
                //检查这个名称是否存在于 allProcedureTypes 列表中
                if (!allProcedureTypes.Contains(procedureTypeName))
                {
                    //如果不存在，则使用 DeleteArrayElementAtIndex 方法从数组中删除该元素
                    proceduresProperty.DeleteArrayElementAtIndex(i);
                }
            }
            //调用 ApplyModifiedProperties 方法将所做的所有更改应用到原始对象上。
            //这通常是在修改序列化对象之后需要调用的，以确保修改被保存并反映到Unity编辑器中的实际对象上
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            ProcedureModule procedureModule = this.target as ProcedureModule;
            procedureModule.value = EditorGUILayout.IntField("value", procedureModule.value);

            //var valuePro=serializedObject.FindProperty("value");
            //valuePro.intValue= EditorGUILayout.IntField("value", valuePro.intValue);

            //游戏运行中 将所有UI元素设置为禁用状态 防止运行时修改组件属性
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            {
                //如果 allProcedureTypes 列表中有元素（即存在可用的Procedure类型），则使用垂直布局显示它们，并为每个类型提供一个切换按钮
                if (allProcedureTypes.Count > 0)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    {
                        for (int i = 0; i < allProcedureTypes.Count; i++)
                        {
                            GUI.changed = false;
                            //查找当前Procedure类型在另一个集合或数组中的索引
                            int? index = FindProcedureTypeIndex(allProcedureTypes[i]);
                            //EditorGUILayout.ToggleLeft 创建一个带标签的切换按钮，并返回当前状态（选中或未选中）
                            bool selected = EditorGUILayout.ToggleLeft(allProcedureTypes[i], index.HasValue);
                            //如果切换按钮的状态发生变化（通过 GUI.changed 判断），则根据新的状态添加或删除相应的Procedure
                            if (GUI.changed)
                            {
                                if (selected)
                                {
                                    AddProcedure(allProcedureTypes[i]);
                                }
                                else
                                {
                                    RemoveProcedure(index.Value);
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUI.EndDisabledGroup();

            if (proceduresProperty.arraySize == 0)
            {
                if (allProcedureTypes.Count == 0)
                {
                    EditorGUILayout.HelpBox("Can't find any procedure", UnityEditor.MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select a procedure at least", UnityEditor.MessageType.Info);
                }
            }
            else
            {
                //如果游戏运行中
                if (Application.isPlaying)
                {
                    //播放中显示当前状态
                    // 则显示当前激活的Procedure的完整类型名
                    EditorGUILayout.LabelField("Current Procedure", TGameFramework.Instance.GetModule<ProcedureModule>().CurrentProcedure?.GetType().FullName);
                }
                else
                {
                    // 如果Unity编辑器不在播放模式
                    // 则显示一个下拉列表，允许用户选择默认的Procedure

                    //显示默认状态
                    // 创建一个列表来存储所有已选择的Procedure类型名
                    List<string> selectedProcedures = new List<string>();
                    for (int i = 0; i < proceduresProperty.arraySize; i++)
                    {
                        // 遍历proceduresProperty数组，并将每个元素的字符串值添加到selectedProcedures列表中
                        selectedProcedures.Add(proceduresProperty.GetArrayElementAtIndex(i).stringValue);
                    }
                    // 对selectedProcedures列表进行排序，这样下拉列表中的选项就会按字母顺序排列
                    selectedProcedures.Sort();

                    // 查找默认Procedure的索引
                    int defaultProcedureIndex = selectedProcedures.IndexOf(defaultProcedureProperty.stringValue);

                    // 使用EditorGUILayout.Popup显示一个下拉列表，让用户选择默认的Procedure
                    // 下拉列表的标题是"Default Procedure"，当前选中的索引是defaultProcedureIndex
                    // 下拉列表的选项是selectedProcedures数组中的元素
                    defaultProcedureIndex = EditorGUILayout.Popup("Default Procedure", defaultProcedureIndex, selectedProcedures.ToArray());

                    // 如果用户在下拉列表中选择了一个选项（即defaultProcedureIndex不是负数）
                    if (defaultProcedureIndex >= 0)
                    {
                        // 更新defaultProcedureProperty的字符串值为用户选择的Procedure类型名
                        defaultProcedureProperty.stringValue = selectedProcedures[defaultProcedureIndex];
                    }
                }
            }

            //每次调用 更改方法 后，都需要调用 serializedObject.ApplyModifiedProperties() 来确保更改被应用到目标对象上
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="procedureType">类型名</param>
        private void AddProcedure(string procedureType)
        {
            // 在数组的起始位置（索引0）插入一个新的元素
            proceduresProperty.InsertArrayElementAtIndex(0);

            // 将新插入的元素（现在位于索引0）的字符串值设置为传入的 procedureType
            //GetArrayElementAtIndex(0) 方法用于获取数组中位于指定索引的元素
            proceduresProperty.GetArrayElementAtIndex(0).stringValue = procedureType;
        }

        private void RemoveProcedure(int index)
        {
            // 获取指定索引位置的 procedure 的类型名称
            string procedureType = proceduresProperty.GetArrayElementAtIndex(index).stringValue;
            // 检查要移除的 procedure 是否是默认 procedure
            if (procedureType == defaultProcedureProperty.stringValue)
            {
                // 如果是默认 procedure，则记录一个警告并返回，不执行移除操作
                Debug.LogWarning("Can't remove default procedure");
                return;
            }

            //DeleteArrayElementAtIndex(index) 方法从 proceduresProperty 数组中删除指定索引位置的元素
            proceduresProperty.DeleteArrayElementAtIndex(index);
        }
        /// <summary>
        /// 查找返回下标
        /// </summary>
        /// <param name="procedureType">表示要查找的 "procedure" 类型名称</param>
        /// <returns></returns>
        private int? FindProcedureTypeIndex(string procedureType)
        {
            for (int i = 0; i < proceduresProperty.arraySize; i++)
            {
                //在每次循环中，通过 GetArrayElementAtIndex(i) 方法获取当前索引 i 处的 SerializedProperty 对象，并将其存储在变量 p 中
                SerializedProperty p = proceduresProperty.GetArrayElementAtIndex(i);
                if (p.stringValue == procedureType)
                {
                    //如果找到匹配的 procedureType，则立即返回当前的索引 i
                    return i;
                }
            }
            //如果遍历完整个数组都没有找到匹配的 procedureType，则最终返回 null
            return null;
        }
    }
}