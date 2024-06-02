using System;

 
    public interface IAwakeSystem : ISystem//�ӿڼ̳нӿ� ֻ�Ǽ̳ж��壬����Ҫʵ�ֽӿ���ķ��� 
    {
        Type ComponentType();
    }

    [ECSSystem]
    public abstract class AwakeSystem<C> : IAwakeSystem where C : ECSComponent//��ͽṹ��̳нӿڣ���Ҫʵ�ֽӿ���ķ���
{
        public abstract void Awake(C c);

        public Type ComponentType()
        {
            return typeof(C);
        }

        public Type SystemType()
        {
            return GetType();
        }
    }

    [ECSSystem]
    public abstract class AwakeSystem<C, P1> : IAwakeSystem where C : ECSComponent
    {
        public abstract void Awake(C c, P1 p1);

        public Type ComponentType()
        {
            return typeof(C);
        }

        public Type SystemType()
        {
            return GetType();
        }
    }


    [ECSSystem]
    public abstract class AwakeSystem<C, P1, P2> : IAwakeSystem where C : ECSComponent
    {
        public abstract void Awake(C c, P1 p1, P2 p2);

        public Type ComponentType()
        {
            return typeof(C);
        }

        public Type SystemType()
        {
            return GetType();
        }
    }
 
