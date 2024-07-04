using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class ItemGenerationSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld _world;
        private EcsFilter _generatorFilter;
        private EcsFilter _itemFilter;

        private EcsPool<ItemGeneratorComponenet> _itemGeneratorPool;
        private EcsPool<StorageComponent> _storagePool;
        private EcsPool<TimerComponenet> _timerPool;
        private EcsPool<SpawnPointComponent> _spawnPointPool;
        private EcsPool<ItemComponent> _itemPool;
        private EcsPool<SizeComponent> _sizePool;
        private EcsPool<TransformComponenet> _transformPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _generatorFilter = _world.Filter<ItemGeneratorComponenet>()
                .Inc<StorageComponent>()
                .Inc<TimerComponenet>()
                .Inc<SpawnPointComponent>()
                .End();

            _itemFilter = _world.Filter<ItemComponent>()
                .Inc<TransformComponenet>()
                .Inc<SizeComponent>()
                .End();

            _itemGeneratorPool = _world.GetPool<ItemGeneratorComponenet>();
            _storagePool = _world.GetPool<StorageComponent>();
            _timerPool = _world.GetPool<TimerComponenet>();
            _spawnPointPool = _world.GetPool<SpawnPointComponent>();
            _itemPool = _world.GetPool<ItemComponent>();
            _sizePool = _world.GetPool<SizeComponent>();
            _transformPool = _world.GetPool<TransformComponenet>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _generatorFilter)
            {
                ref StorageComponent storage = ref _storagePool.Get(entity);

                if (storage.IsFull)
                    continue;

                ref TimerComponenet timer = ref _timerPool.Get(entity);

                if (timer.Elapsed)
                {
                    SpawnItem(entity, ref storage);
                    timer.Reset();
                }
                else
                {
                    timer.AddTime(Time.deltaTime);
                }

            }
        }

        private void SpawnItem(int entity, ref StorageComponent storage)
        {
            ref ItemGeneratorComponenet generator = ref _itemGeneratorPool.Get(entity);
            ref SpawnPointComponent spawnPoint = ref _spawnPointPool.Get(entity);

            ItemConverter spawnedItem = Object.Instantiate(generator.prefab);
            EcsConverter.ConvertObject(spawnedItem.gameObject, _world);

            int itemEntity = _itemFilter.GetRawEntities()[_itemFilter.GetEntitiesCount() - 1];

            ref SizeComponent size = ref _sizePool.Get(itemEntity);
            ref TransformComponenet itemTransform = ref _transformPool.Get(itemEntity);
            ref TransformComponenet storageTransfrom = ref _transformPool.Get(entity);

            itemTransform.value.parent = storageTransfrom.value;

            Vector3 spawnPos = spawnPoint.point.position + (0.5f * size.value.y * Vector3.up) + storage.CalculateNewItemPos(size.value);
            itemTransform.value.position = spawnPos;

            ref ItemComponent item = ref _itemPool.Get(itemEntity);
            storage.Add(item);
        }
    }
}