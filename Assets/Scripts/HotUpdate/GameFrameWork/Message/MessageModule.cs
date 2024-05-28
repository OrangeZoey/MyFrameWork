using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class MessageModule : BaseGameModule
{
    //定义委托  返回值为Task    参数为T
    public delegate Task MessageHandlerEventArgs<T>(T arg);

    private Dictionary<Type, List<object>> globalMessageHandlers;//全局的消息处理器
    private Dictionary<Type, List<object>> localMessageHandlers;//本地的消息处理器

    public Monitor Monitor { get; private set; }//监听

    /// <summary>
    /// 初始化操作
    /// </summary>
    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();

        localMessageHandlers = new Dictionary<Type, List<object>>();
        Monitor = new Monitor();
        LoadAllMessageHandler();
    }

    /// <summary>
    /// 停止操作
    /// </summary>
    protected internal override void OnModuleStop()
    {
        base.OnModuleStop();
        globalMessageHandlers = null;
        localMessageHandlers = null;
    }


    /// <summary>
    /// 加载所有的消息处理器
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void LoadAllMessageHandler()
    {
        globalMessageHandlers = new Dictionary<Type, List<object>>();

        // Assembly.GetCallingAssembly().GetTypes()  获取程序集中所有类型
        foreach (var type in Assembly.GetCallingAssembly().GetTypes())
        {
            if (type.IsAbstract)//是抽象类型 继续循环
                continue;
            
            //检查是否含有该特性
            MessageHandlerAttribute messageHandlerAttribute=type.GetCustomAttribute<MessageHandlerAttribute>();

            //如果有该特性
            if (messageHandlerAttribute!=null)
            {
                //Activator.CreateInstance .Net框架中的方法 用于创建指定类型的实例
                //这里创建了 IMessageHander 的实例
                IMessageHander messageHander =Activator.CreateInstance(type) as IMessageHander;

                //判断字典中的键是否包含该类型
                if (!globalMessageHandlers.ContainsKey(messageHander.GetHandlerType()))
                {
                    globalMessageHandlers.Add(messageHander.GetHandlerType(), new List<object>());
                }

                //添加进字典键对应的集合
                globalMessageHandlers[messageHander.GetHandlerType()].Add(messageHander);
            }
        }
    }


    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    public void Subscribe<T>(MessageHandlerEventArgs<T> handler)
    {
        //获取消息类型
        Type argType=typeof(T);

        //字典中获取处理器列表
        if (!localMessageHandlers.TryGetValue(argType,out var handlerList))
        {
            handlerList=new List<object> (); //不存在 创建
            localMessageHandlers.Add(argType, handlerList);//添加进字典
        }

        //将传入的对象添加到列表中
        handlerList.Add(handler);
    }


    /// <summary>
    /// 移除消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    public void Unsubscribe<T>(MessageHandlerEventArgs<T> handler)
    {
        //判断是否存在
        if (!localMessageHandlers.TryGetValue(typeof(T), out var handlerList))
            return;

        //存在 移除
        handlerList.Remove(handler);
    }


    /// <summary>
    /// 发送消息 （发送消息 返回Task）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg"></param>
    /// <returns></returns>
    public async Task Post<T>(T arg)where T:struct
    {
        //此类型消息是否在全局消息处理器中
        if (globalMessageHandlers.TryGetValue(typeof(T), out List<object> globalHandlerList))
        {
            //如果在  取出集合
            foreach (var handler in globalHandlerList)
            {
                //是一个类型模式匹配(type pattern matching)的语法，用于判断一个对象是否属于指定类型，并将其转换为该类型的实例
                //在给定的代码中，handler是一个对象 而MessageHandler<T>是一个泛型类型。该语句可以解读为：
                //handler is MessageHandler<T> ：判断handler对象是否是类型MessageHandler<T> 的实例
                //messageHandler ：如果判断为真 将handler对象转换为MessageHandler<T> 类型，并将其赋值给变量 messageHandler
                if (!(handler is MessageHandler<T> messageHandler))
                    continue;

                //等待消息处理结果
                await messageHandler.HandleMessage(arg);
            }
        }

        //本迪消息处理器
        if (localMessageHandlers.TryGetValue(typeof(T), out List<object> localHandlerList))
        {
            //从对象池中获取一个列表  优化性能 减少内存分配
            List<object> list = ListPool<object>.Obtain();

            //将本地处理器列表的内容添加到从对象池中获取的列表中
            list.AddRangeNonAlloc(localHandlerList);

            //遍历列表中的每个处理器，并检查它是否为 MessageHandlerEventArgs<T> 委托类型
            foreach (var handler in list)
            {
                if (!(handler is MessageHandlerEventArgs<T> messageHandler))
                    continue;

                //处理器是 MessageHandlerEventArgs<T> 类型，则直接调用该委托并传入 arg 参数
                await messageHandler(arg);
            }

            //处理完所有本地处理器后，使用 ListPool<object>.Release(list) 将列表释放回对象池
            ListPool<object>.Release(list);

        }

    }
}
