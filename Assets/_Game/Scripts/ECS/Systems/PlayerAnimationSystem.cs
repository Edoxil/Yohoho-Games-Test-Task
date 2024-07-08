using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class PlayerAnimationSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;

        private EcsPool<AnimatorComponenet> _animatorPool;
        private EcsPool<DirectionComponenet> _directionPool;
        private EcsPool<StorageComponent> _storagePool;

        private int _isMovingHash = Animator.StringToHash("IsMoving");
        private int _handsLayerIndex = 1;

        private bool _isMoving;

        private float _targetHandsLayerWeight;
        private float _currentHandsLayerWeight;
        private float _actualHandsLayerWeight;

        private float _weightChangeSpeed = 10f;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _filter = world.Filter<PlayerTag>()
                .Inc<DirectionComponenet>()
                .Inc<AnimatorComponenet>()
                .Inc<StorageComponent>()
                .End();

            _animatorPool = world.GetPool<AnimatorComponenet>();
            _directionPool = world.GetPool<DirectionComponenet>();
            _storagePool = world.GetPool<StorageComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            if (_filter.GetEntitiesCount() <= 0)
                return;

            int playerID = _filter.GetRawEntities()[0];
            ref AnimatorComponenet animator = ref _animatorPool.Get(playerID);

            UpdateMovingState(playerID, animator);
            UpdateCarryingState(playerID, animator);
        }

        private void UpdateCarryingState(int playerID, AnimatorComponenet animator)
        {
            ref StorageComponent storage = ref _storagePool.Get(playerID);
            _targetHandsLayerWeight = storage.HasItems ? 1f : 0f;
            _currentHandsLayerWeight = animator.value.GetLayerWeight(_handsLayerIndex);
            _actualHandsLayerWeight = Mathf.Lerp(_currentHandsLayerWeight, _targetHandsLayerWeight, _weightChangeSpeed * Time.deltaTime);
            animator.value.SetLayerWeight(_handsLayerIndex, _actualHandsLayerWeight);
        }

        private void UpdateMovingState(int playerID, AnimatorComponenet animator)
        {
            ref DirectionComponenet direction = ref _directionPool.Get(playerID);

            _isMoving = direction.value.magnitude > 0f;
            animator.value.SetBool(_isMovingHash, _isMoving);
        }
    }
}