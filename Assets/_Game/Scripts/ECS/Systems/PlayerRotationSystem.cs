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
            if (_filter.GetEntitiesCount() <= 0)
                return;

            int playerID = _filter.GetRawEntities()[0];
            ref DirectionComponenet direction = ref _directionPool.Get(playerID);

            if (direction.value == Vector3.zero)
                return;

            ref TransformComponenet transform = ref _transformPool.Get(playerID);
            ref RotationSpeedComponent rotSpeed = ref _rotationSpeedPool.Get(playerID);

            _lookDirection = Quaternion.LookRotation(direction.value, Vector3.up);
            _lookDirection.x = 0f;
            _lookDirection.z = 0f;

            transform.value.rotation = Quaternion.Lerp(transform.value.rotation, _lookDirection,
                   rotSpeed.value * Time.deltaTime);
        }
    }
}