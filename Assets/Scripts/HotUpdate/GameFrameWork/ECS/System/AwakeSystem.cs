using System;

 
    public interface IAwakeSystem : ISystem//接口继承接口 只是继承定义，不需要实现接口里的方法 
    {
        Type ComponentType();
    }

    [ECSSystem]
    public abstract class AwakeSystem<C> : IAwakeSystem where C : ECSComponent//类和结构体继承接口，需要实现接口里的方法
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
 
