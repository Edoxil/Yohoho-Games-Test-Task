using Leopotam.EcsLite;

namespace Game
{
    public sealed class StorageInitializationSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsPool<StorageComponent> _storagePool;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _storagePool = world.GetPool<StorageComponent>();

            _filter = world.Filter<StorageComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref StorageComponent storage = ref _storagePool.Get(entity);

                if (storage.Initialized == false)
                    storage.Init();
            }
        }
    }
}