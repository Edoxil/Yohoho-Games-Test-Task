using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class CameraFollowSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _cameraFilter;
        private EcsFilter _targetFilter;

        private EcsPool<TransformComponenet> _transformPool;
        private EcsPool<SpeedComponent> _speedPool;
        private EcsPool<OffsetComponenet> _offsetPool;

        public void Init(IEcsSystems systems)
        {
            EcsWorld world = systems.GetWorld();

            _cameraFilter = world.Filter<CameraTag>()
                .Inc<TransformComponenet>()
                .Inc<SpeedComponent>()
                .Inc<OffsetComponenet>()
                .End();

            _targetFilter = world.Filter<CameraTargetTag>()
                .Inc<TransformComponenet>()
                .End();

            _transformPool = world.GetPool<TransformComponenet>();
            _speedPool = world.GetPool<SpeedComponent>();
            _offsetPool = world.GetPool<OffsetComponenet>();
        }

        public void Run(IEcsSystems systems)
        {
            if (_cameraFilter.GetEntitiesCount() <= 0)
                return;

            if (_targetFilter.GetEntitiesCount() <= 0)
                return;

            int cameraID = _cameraFilter.GetRawEntities()[0];
            int targetID = _targetFilter.GetRawEntities()[0];

            ref TransformComponenet cameraTransform = ref _transformPool.Get(cameraID);
            ref TransformComponenet tartgetTransform = ref _transformPool.Get(targetID);

            ref SpeedComponent speed = ref _speedPool.Get(cameraID);
            ref OffsetComponenet offset = ref _offsetPool.Get(cameraID);

            cameraTransform.value.position = Vector3.Lerp(cameraTransform.value.position, tartgetTransform.value.position + offset.value,
                 speed.value * Time.deltaTime);

        }
    }
}