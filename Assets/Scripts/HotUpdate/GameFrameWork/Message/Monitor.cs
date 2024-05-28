using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Monitor
{
    //存储不同类型的等待对象
    private readonly Dictionary<Type, object> waitObjects = new Dictionary<Type, object>();

    /// <summary>
    /// 存储等待对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public WaitObject<T> Wait<T>() where T : struct
    {
        WaitObject<T> obj = new WaitObject<T>(); //创建 WaitObject<T>类型的实例obj
        waitObjects.Add(typeof(T), obj);        //添加进字典
        return obj;
    }

    /// <summary>
    /// 设置等待对象的结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    public void SeResult<T>(T result) where T : struct
    {
        Type type = typeof(T);      //先获取Type对象
        if (!waitObjects.TryGetValue(type, out object obj))  //判断字典中是否存在
            return;

        waitObjects.Remove(type);                   //设置了结果 不必等待了
        ((WaitObject<T>)obj).SetResult(result);     //设置结果
    }



    //INotifyCompletion 实现这个接口的实例
    public class WaitObject<T> : INotifyCompletion where T : struct
    {
        //Awaiter对象
        //必须继承INotifyCompletion接口，并实现其中的OnCompleted(Action continuation)方法
        //必须包含IsCompleted属性
        //必须包含GetResult()方法

        //是否完成
        public bool IsCompleted { get; private set; }

        //结果
        public T Result { get; private set; }

        //回调
        private Action callback;

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(T result)
        {
            Result = result;//设置结果
            IsCompleted = true;//标记为已完成

            Action c = callback;//防止回调执行过程中 callback被修改 先赋值给局部变量
            callback = null;
            c?.Invoke();
        }

        /// <summary>
        ///返回等待者本身
        /// </summary>
        /// <returns></returns>
        public WaitObject<T> GetAwaiter()
        {
            //INotifyCompletion 主要用于异步编程中的 await 表达式。
            //当一个对象被 await 时，它必须提供一个 GetAwaiter 方法，而返回的 Awaiter 对象必须实现 INotifyCompletion 接口
            return this;
        }


        /// <summary>
        /// 接口一部分
        /// </summary>
        /// <param name="callback"></param>
        public void OnCompleted(Action callback)
        {
            this.callback = callback;  //用于注册一个回调函数
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <returns></returns>
        public T GetResult()
        {
            return this.Result;
        }
    }

}
