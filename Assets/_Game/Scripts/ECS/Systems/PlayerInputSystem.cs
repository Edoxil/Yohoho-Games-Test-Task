using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SimpleInputNamespace;
using UnityEngine;

namespace Game
{
    public sealed class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;
        private EcsPool<DirectionComponenet> _directionPool;

        private Joystick _joystick;
        private Vector3 _inputDirection;

        private readonly EcsCustomInject<SceneData> _sceneData;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _filter = world.Filter<PlayerTag>()
                .Inc<DirectionComponenet>()
                .End();

            _directionPool = world.GetPool<DirectionComponenet>();

            _joystick = _sceneData.Value.Joystick;
        }

        public void Run(IEcsSystems systems)
        {
            if (_filter.GetEntitiesCount() <= 0)
                return;

            int playerID = _filter.GetRawEntities()[0];

            _inputDirection.x = _joystick.Value.x;
            _inputDirection.z = _joystick.Value.y;


            ref DirectionComponenet direction = ref _directionPool.Get(playerID);
            direction.value = _inputDirection;
        }
    }
}