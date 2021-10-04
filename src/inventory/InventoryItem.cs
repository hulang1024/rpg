namespace InventorySystem
{
    public class InventoryItem
    {
        public long Id;
        public int slotRow;
        public int slotCol;
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
