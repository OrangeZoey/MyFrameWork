 
    public class ItemInfo
    {
        public long id;
        public ItemType type;
        public EquipmentInfo equipmentInfo;

        public override string ToString()
        {
            switch (type)
            {
                case ItemType.Equipment:
                    return equipmentInfo.ToString();
                case ItemType.Prop:
                default:
                    return null;
            }
        }
    }
 
