using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class PlayerAnimationSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;

        private EcsPool<AnimatorComponenet> _animatorPool;
        private EcsPool<DirectionComponenet> _directionPool;

        private int _isMovingHash = Animator.StringToHash("IsMoving");
        private bool _isMoving;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _filter = world.Filter<PlayerTag>()
                .Inc<DirectionComponenet>()
                .Inc<AnimatorComponenet>()
                .End();

            _animatorPool = world.GetPool<AnimatorComponenet>();
            _directionPool = world.GetPool<DirectionComponenet>();
        }

        public void Run(IEcsSystems systems)
        {
            if (_filter.GetEntitiesCount() <= 0)
                return;
                
            int playerID = _filter.GetRawEntities()[0];

            ref AnimatorComponenet animator = ref _animatorPool.Get(playerID);
            ref DirectionComponenet direction = ref _directionPool.Get(playerID);

            _isMoving = direction.value.magnitude > 0f;
            animator.value.SetBool(_isMovingHash, _isMoving);
        }
    }
}