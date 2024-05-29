using System.Collections.Generic;

public class KnapsackComponent : ECSComponent
{
    public Dictionary<long, ItemInfo> items;
}

public class KnapsackComponentAwakeSystem : AwakeSystem<KnapsackComponent>
{
    public override void Awake(KnapsackComponent c)
    {
        c.items = DictionaryPool<long, ItemInfo>.Obtain();
        c.items.Add(111, new ItemInfo() { id = 10002, type = ItemType.Equipment });
    }
}

public class KnapsackComponentDestroySystem : DestroySystem<KnapsackComponent>
{
    public override void Destroy(KnapsackComponent c)
    {
        DictionaryPool<long, ItemInfo>.Release(c.items);
        c.items = null;
    }
}

 

///// <summary>
///// 拾取物品
///// </summary>
//public class PickupItemMessageHandler : MessageHandler<MessageType.PickupItem>
//{
//    public override async Task HandleMessage(MessageType.PickupItem arg)
//    {
//        KnapsackComponent knapsackComponent = GameManager.ECS.World.GetComponent<KnapsackComponent>();
//        if (knapsackComponent == null)
//            return;

//        ECSEntity entity = GameManager.ECS.FindEntity(arg.entityID);
//        if (entity == null)
//            return;

//        DropItemComponent dropItemComponent = entity.GetComponent<DropItemComponent>();
//        if (dropItemComponent == null)
//            return;

//        ItemInfo itemInfo = dropItemComponent.itemInfo;
//        UnityLog.Info($"Pick drop item:{itemInfo}");

//        knapsackComponent.items.Add(itemInfo.id, itemInfo);
//        entity.Dispose();

//        // 穿装备
//        await GameManager.Message.Post(new MessageType.WearEqupmentRequest()
//        {
//            itemID = itemInfo.id,
//        });
//    }
//}

