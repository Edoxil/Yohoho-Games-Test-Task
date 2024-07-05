using Leopotam.EcsLite;

namespace Game
{
    public class Item
    {
        public ItemType ItemType { get; private set; }
        public EcsPackedEntity PackedEntity { get; private set; }

        public Item(ItemType itemType, EcsPackedEntity packedEntity)
        {
            ItemType = itemType;
            PackedEntity = packedEntity;
        }
    }
}