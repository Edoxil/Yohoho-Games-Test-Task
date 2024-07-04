using Leopotam.EcsLite;

namespace Game
{
    [System.Serializable]
    public struct ItemTransactionComponent
    {
        public EcsPackedEntity storage;
        public EcsPackedEntity item;
    }
}