using Leopotam.EcsLite;
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

        public PlayerInputSystem(Joystick joystick)
        {
            _joystick = joystick;
        }

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _filter = world.Filter<PlayerTag>()
                .Inc<DirectionComponenet>()
                .End();

            _directionPool = world.GetPool<DirectionComponenet>();
        }

        public void Run(IEcsSystems systems)
        {
            _inputDirection.x = _joystick.Value.x;
            _inputDirection.z = _joystick.Value.y;

            foreach (var entity in _filter)
            {
                ref DirectionComponenet direction = ref _directionPool.Get(entity);
                direction.value = _inputDirection;
            }

        }
    }
}