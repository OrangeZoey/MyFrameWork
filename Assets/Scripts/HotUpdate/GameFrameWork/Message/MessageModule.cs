using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public class MessageModule : BaseGameModule
{
    //����ί��  ����ֵΪTask    ����ΪT
    public delegate Task MessageHandlerEventArgs<T>(T arg);

    private Dictionary<Type, List<object>> globalMessageHandlers;//ȫ�ֵ���Ϣ������
    private Dictionary<Type, List<object>> localMessageHandlers;//���ص���Ϣ������

    public Monitor Monitor { get; private set; }//����

    /// <summary>
    /// ��ʼ������
    /// </summary>
    protected internal override void OnModuleInit()
    {
        base.OnModuleInit();

        localMessageHandlers = new Dictionary<Type, List<object>>();
        Monitor = new Monitor();
        LoadAllMessageHandler();
    }

    /// <summary>
    /// ֹͣ����
    /// </summary>
    protected internal override void OnModuleStop()
    {
        base.OnModuleStop();
        globalMessageHandlers = null;
        localMessageHandlers = null;
    }


    /// <summary>
    /// �������е���Ϣ������
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void LoadAllMessageHandler()
    {
        globalMessageHandlers = new Dictionary<Type, List<object>>();

        // Assembly.GetCallingAssembly().GetTypes()  ��ȡ��������������
        foreach (var type in Assembly.GetCallingAssembly().GetTypes())
        {
            if (type.IsAbstract)//�ǳ������� ����ѭ��
                continue;
            
            //����Ƿ��и�����
            MessageHandlerAttribute messageHandlerAttribute=type.GetCustomAttribute<MessageHandlerAttribute>();

            //����и�����
            if (messageHandlerAttribute!=null)
            {
                //Activator.CreateInstance .Net����еķ��� ���ڴ���ָ�����͵�ʵ��
                //���ﴴ���� IMessageHander ��ʵ��
                IMessageHander messageHander =Activator.CreateInstance(type) as IMessageHander;

                //�ж��ֵ��еļ��Ƿ����������
                if (!globalMessageHandlers.ContainsKey(messageHander.GetHandlerType()))
                {
                    globalMessageHandlers.Add(messageHander.GetHandlerType(), new List<object>());
                }

                //��ӽ��ֵ����Ӧ�ļ���
                globalMessageHandlers[messageHander.GetHandlerType()].Add(messageHander);
            }
        }
    }


    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    public void Subscribe<T>(MessageHandlerEventArgs<T> handler)
    {
        //��ȡ��Ϣ����
        Type argType=typeof(T);

        //�ֵ��л�ȡ�������б�
        if (!localMessageHandlers.TryGetValue(argType,out var handlerList))
        {
            handlerList=new List<object> (); //������ ����
            localMessageHandlers.Add(argType, handlerList);//��ӽ��ֵ�
        }

        //������Ķ�����ӵ��б���
        handlerList.Add(handler);
    }


    /// <summary>
    /// �Ƴ���Ϣ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    public void Unsubscribe<T>(MessageHandlerEventArgs<T> handler)
    {
        //�ж��Ƿ����
        if (!localMessageHandlers.TryGetValue(typeof(T), out var handlerList))
            return;

        //���� �Ƴ�
        handlerList.Remove(handler);
    }


    /// <summary>
    /// ������Ϣ ��������Ϣ ����Task��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg"></param>
    /// <returns></returns>
    public async Task Post<T>(T arg)where T:struct
    {
        //��������Ϣ�Ƿ���ȫ����Ϣ��������
        if (globalMessageHandlers.TryGetValue(typeof(T), out List<object> globalHandlerList))
        {
            //�����  ȡ������
            foreach (var handler in globalHandlerList)
            {
                //��һ������ģʽƥ��(type pattern matching)���﷨�������ж�һ�������Ƿ�����ָ�����ͣ�������ת��Ϊ�����͵�ʵ��
                //�ڸ����Ĵ����У�handler��һ������ ��MessageHandler<T>��һ���������͡��������Խ��Ϊ��
                //handler is MessageHandler<T> ���ж�handler�����Ƿ�������MessageHandler<T> ��ʵ��
                //messageHandler ������ж�Ϊ�� ��handler����ת��ΪMessageHandler<T> ���ͣ������丳ֵ������ messageHandler
                if (!(handler is MessageHandler<T> messageHandler))
                    continue;

                //�ȴ���Ϣ������
                await messageHandler.HandleMessage(arg);
            }
        }

        //������Ϣ������
        if (localMessageHandlers.TryGetValue(typeof(T), out List<object> localHandlerList))
        {
            //�Ӷ�����л�ȡһ���б�  �Ż����� �����ڴ����
            List<object> list = ListPool<object>.Obtain();

            //�����ش������б��������ӵ��Ӷ�����л�ȡ���б���
            list.AddRangeNonAlloc(localHandlerList);

            //�����б��е�ÿ������������������Ƿ�Ϊ MessageHandlerEventArgs<T> ί������
            foreach (var handler in list)
            {
                if (!(handler is MessageHandlerEventArgs<T> messageHandler))
                    continue;

                //�������� MessageHandlerEventArgs<T> ���ͣ���ֱ�ӵ��ø�ί�в����� arg ����
                await messageHandler(arg);
            }

            //���������б��ش�������ʹ�� ListPool<object>.Release(list) ���б��ͷŻض����
            ListPool<object>.Release(list);

        }

    }
}
