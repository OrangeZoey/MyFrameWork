using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Monitor
{
    //�洢��ͬ���͵ĵȴ�����
    private readonly Dictionary<Type, object> waitObjects = new Dictionary<Type, object>();

    /// <summary>
    /// �洢�ȴ�����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public WaitObject<T> Wait<T>() where T : struct
    {
        WaitObject<T> obj = new WaitObject<T>(); //���� WaitObject<T>���͵�ʵ��obj
        waitObjects.Add(typeof(T), obj);        //��ӽ��ֵ�
        return obj;
    }

    /// <summary>
    /// ���õȴ�����Ľ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    public void SeResult<T>(T result) where T : struct
    {
        Type type = typeof(T);      //�Ȼ�ȡType����
        if (!waitObjects.TryGetValue(type, out object obj))  //�ж��ֵ����Ƿ����
            return;

        waitObjects.Remove(type);                   //�����˽�� ���صȴ���
        ((WaitObject<T>)obj).SetResult(result);     //���ý��
    }



    //INotifyCompletion ʵ������ӿڵ�ʵ��
    public class WaitObject<T> : INotifyCompletion where T : struct
    {
        //Awaiter����
        //����̳�INotifyCompletion�ӿڣ���ʵ�����е�OnCompleted(Action continuation)����
        //�������IsCompleted����
        //�������GetResult()����

        //�Ƿ����
        public bool IsCompleted { get; private set; }

        //���
        public T Result { get; private set; }

        //�ص�
        private Action callback;

        /// <summary>
        /// ���ý��
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(T result)
        {
            Result = result;//���ý��
            IsCompleted = true;//���Ϊ�����

            Action c = callback;//��ֹ�ص�ִ�й����� callback���޸� �ȸ�ֵ���ֲ�����
            callback = null;
            c?.Invoke();
        }

        /// <summary>
        ///���صȴ��߱���
        /// </summary>
        /// <returns></returns>
        public WaitObject<T> GetAwaiter()
        {
            //INotifyCompletion ��Ҫ�����첽����е� await ���ʽ��
            //��һ������ await ʱ���������ṩһ�� GetAwaiter �����������ص� Awaiter �������ʵ�� INotifyCompletion �ӿ�
            return this;
        }


        /// <summary>
        /// �ӿ�һ����
        /// </summary>
        /// <param name="callback"></param>
        public void OnCompleted(Action callback)
        {
            this.callback = callback;  //����ע��һ���ص�����
        }

        /// <summary>
        /// ��ȡ���
        /// </summary>
        /// <returns></returns>
        public T GetResult()
        {
            return this.Result;
        }
    }

}
