using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模块基类
/// </summary>
public class BaseGameModule : MonoBehaviour
{
    private void Awake() { }
    private void Start() { }
    private void Update() { }
    private void OnDestroy() { }

    /// <summary>
    /// 模块初始化
    /// </summary>
    protected internal virtual void OnModuleInit() { }

    /// <summary>
    /// 启动模块
    /// </summary>
    protected internal virtual void OnModuleStart() { }

    /// <summary>
    /// 停止模块
    /// </summary>
    protected internal virtual void OnModuleStop() { }

    /// <summary>
    /// 模块的Update
    /// </summary>
    /// <param name="deltaTime"></param>
    protected internal virtual void OnModuleUpdate(float deltaTime) { }

    /// <summary>
    /// 模块的LateUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    protected internal virtual void OnModuleLateUpdate(float deltaTime) { }

    /// <summary>
    /// 模块的FixedUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    protected internal virtual void OnModuleFixedUpdate(float deltaTime) { }
}
