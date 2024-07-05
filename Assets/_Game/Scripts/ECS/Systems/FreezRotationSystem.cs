using Leopotam.EcsLite;

namespace Game
{
    public sealed class FreezRotationSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;

        private EcsPool<FreezRotationComponenet> _freezRotationPool;
        private EcsPool<TransformComponenet> _transformPool;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _filter = world.Filter<FreezRotationComponenet>()
                .Inc<TransformComponenet>()
                .End();

            _freezRotationPool = world.GetPool<FreezRotationComponenet>();
            _transformPool = world.GetPool<TransformComponenet>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref TransformComponenet transform = ref _transformPool.Get(entity);
                ref FreezRotationComponenet freezRotation = ref _freezRotationPool.Get(entity);

                transform.value.eulerAngles = freezRotation.value;

            }
        }
    }
}