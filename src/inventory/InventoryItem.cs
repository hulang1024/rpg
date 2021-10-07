namespace InventorySystem
{
    public class InventoryItem
    {
        public long Id;
        public int SlotRow;
        public int SlotCol;
        public long CategoryId;
        public long ObjectId;
        public ItemObjectType ObjectType;
        public int Quantity;
    }

    public enum ItemObjectType
    {
        LockKey
    }
}
