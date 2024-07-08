using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class PlayerMovingSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;

        private EcsPool<DirectionComponenet> _directionPool;
        private EcsPool<SpeedComponent> _speedPool;
        private EcsPool<CharacterControllerComponent> _characterControllerPool;

        private Vector3 _motion;
        private Vector3 _gravity = new Vector3(0f, -9.81f, 0f);

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _filter = world.Filter<PlayerTag>()
                .Inc<DirectionComponenet>()
                .Inc<SpeedComponent>()
                .Inc<CharacterControllerComponent>()
                .End();

            _directionPool = world.GetPool<DirectionComponenet>();
            _speedPool = world.GetPool<SpeedComponent>();
            _characterControllerPool = world.GetPool<CharacterControllerComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (_filter.GetEntitiesCount() <= 0)
                return;

            int playerID = _filter.GetRawEntities()[0];

            ref SpeedComponent speed = ref _speedPool.Get(playerID);
            ref DirectionComponenet direction = ref _directionPool.Get(playerID);
            ref CharacterControllerComponent characterController = ref _characterControllerPool.Get(playerID);

            _motion = speed.value * Time.deltaTime * direction.value + (_gravity * Time.deltaTime);

            characterController.value.Move(_motion);
        }
    }
}