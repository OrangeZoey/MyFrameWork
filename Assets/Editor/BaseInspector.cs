using UnityEditor;

namespace TGame.Editor.Inspector
{
    
	public class BaseInspector : UnityEditor.Editor
    {
        //是否应该绘制基类的Inspector界面
        protected virtual bool DrawBaseGUI { get { return true; } }

        private bool isCompiling = false; //是否编译

        //这是一个受保护且虚拟的空方法，它在每次编辑器更新时被 UpdateEditor 方法调用。
        //这个方法的目的是提供一个钩子，允许子类在编辑器更新时执行自定义逻辑。
        //因为它是虚拟的，所以子类可以重写这个方法来添加它们自己的更新代码。
        protected virtual void OnInspectorUpdateInEditor() { }

        /// <summary>
        /// 当脚本被启用时调用
        /// </summary>
        private void OnEnable()
        {
            OnInspectorEnable();
            //注册了一个更新函数UpdateEditor到EditorApplication.update事件
            EditorApplication.update += UpdateEditor;
        }
        protected virtual void OnInspectorEnable() { }

        /// <summary>
        /// 当脚本被禁用时调用
        /// </summary>
        private void OnDisable()
        {
            //从EditorApplication.update事件中注销了UpdateEditor函数
            EditorApplication.update -= UpdateEditor;
            OnInspectorDisable();
        }
        protected virtual void OnInspectorDisable() { }

        /// <summary>
        /// 更新函数
        /// </summary>
        private void UpdateEditor()
        {
           // 在每次编辑器更新时被调用。它检查是否正在编译脚本，并相应地调用OnCompileStart或OnCompileComplete。
           // 此外，它还调用OnInspectorUpdateInEditor来执行任何自定义的更新逻辑。
            if (!isCompiling && EditorApplication.isCompiling)
            {
                isCompiling = true;
                OnCompileStart();
            }
            else if (isCompiling && !EditorApplication.isCompiling)
            {
                isCompiling = false;
                OnCompileComplete();
            }
            OnInspectorUpdateInEditor();
        }
        /// <summary>
        /// 绘制
        /// </summary>
        public override void OnInspectorGUI()
        {
            //自定义Inspector界面的主要绘制方法。
            //如果DrawBaseGUI为真，它将调用基类的OnInspectorGUI方法来绘制默认的Inspector界面
            if (DrawBaseGUI)
            {
                base.OnInspectorGUI();
            }
        }
        /// <summary>
        /// 编译开始时
        /// </summary>
        protected virtual void OnCompileStart() { }

        /// <summary>
        /// 编译完成时
        /// </summary>
        protected virtual void OnCompileComplete() { }
    }
}