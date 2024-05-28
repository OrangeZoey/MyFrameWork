using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ģ�����
/// </summary>
public class BaseGameModule : MonoBehaviour
{
    private void Awake() { }
    private void Start() { }
    private void Update() { }
    private void OnDestroy() { }

    /// <summary>
    /// ģ���ʼ��
    /// </summary>
    protected internal virtual void OnModuleInit() { }

    /// <summary>
    /// ����ģ��
    /// </summary>
    protected internal virtual void OnModuleStart() { }

    /// <summary>
    /// ֹͣģ��
    /// </summary>
    protected internal virtual void OnModuleStop() { }

    /// <summary>
    /// ģ���Update
    /// </summary>
    /// <param name="deltaTime"></param>
    protected internal virtual void OnModuleUpdate(float deltaTime) { }

    /// <summary>
    /// ģ���LateUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    protected internal virtual void OnModuleLateUpdate(float deltaTime) { }

    /// <summary>
    /// ģ���FixedUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    protected internal virtual void OnModuleFixedUpdate(float deltaTime) { }
}
