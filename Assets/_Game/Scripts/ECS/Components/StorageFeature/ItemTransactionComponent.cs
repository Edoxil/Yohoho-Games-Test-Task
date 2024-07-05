using Leopotam.EcsLite;

namespace Game
{
    [System.Serializable]
    public struct ItemTransactionComponent
    {
        public EcsPackedEntity fromStorage;
        public EcsPackedEntity toStorage;
    }
}