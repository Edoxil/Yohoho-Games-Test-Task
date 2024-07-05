using Leopotam.EcsLite;

namespace Game
{
    public sealed class CapacityDisplaySystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;

        private EcsPool<CapacityDisplayComponent> _capacityDisplayPool;
        private EcsPool<StorageComponent> _storagePool;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _filter = world.Filter<CapacityDisplayComponent>()
                .Inc<StorageComponent>()
                .End();

            _capacityDisplayPool = world.GetPool<CapacityDisplayComponent>();
            _storagePool = world.GetPool<StorageComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref StorageComponent storage = ref _storagePool.Get(entity);
                ref CapacityDisplayComponent display = ref _capacityDisplayPool.Get(entity);

                if (storage.IsFull)
                {
                    display.textField.text = "MAX";
                }
                else
                {
                    int currenItems = storage.ItemsCount;
                    int capacity = storage.Capacity;

                    display.textField.text = $"{currenItems}/{capacity}";
                }
            }
        }
    }
}