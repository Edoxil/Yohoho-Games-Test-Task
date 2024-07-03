using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class PlayerRotationSystem : IEcsRunSystem,IEcsInitSystem
    {
        private EcsFilter _filter;

        private EcsPool<DirectionComponenet> _directionPool;
        private EcsPool<RotationSpeedComponent> _rotationSpeedPool;
        private EcsPool<TransformComponenet> _transformPool;

        private Quaternion _lookDirection;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _filter = world.Filter<PlayerTag>()
                .Inc<DirectionComponenet>()
                .Inc<RotationSpeedComponent>()
                .Inc<TransformComponenet>()
                .End();

            _directionPool = world.GetPool<DirectionComponenet>();
            _rotationSpeedPool = world.GetPool<RotationSpeedComponent>();
            _transformPool = world.GetPool<TransformComponenet>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref DirectionComponenet direction = ref _directionPool.Get(entity);

                if (direction.value == Vector3.zero)
                    continue;

                ref TransformComponenet transform = ref _transformPool.Get(entity);
                ref RotationSpeedComponent rotSpeed = ref _rotationSpeedPool.Get(entity);

                _lookDirection = Quaternion.LookRotation(direction.value, Vector3.up);
                _lookDirection.x = 0f;
                _lookDirection.z = 0f;

                transform.value.rotation = Quaternion.Lerp(transform.value.rotation, _lookDirection,
                       rotSpeed.value * Time.deltaTime);
            }
        }
    }
}