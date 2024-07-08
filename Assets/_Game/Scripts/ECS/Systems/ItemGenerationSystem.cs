using AB_Utility.FromSceneToEntityConverter;
using DG.Tweening;
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
        private EcsPool<ItemComponent> _itemPool;
        private EcsPool<SizeComponent> _sizePool;
        private EcsPool<TransformComponenet> _transformPool;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _generatorFilter = _world.Filter<ItemGeneratorComponenet>()
                .Inc<StorageComponent>()
                .Inc<TimerComponenet>()
                .End();

            _itemFilter = _world.Filter<ItemComponent>()
                .Inc<TransformComponenet>()
                .Inc<SizeComponent>()
                .End();

            _itemGeneratorPool = _world.GetPool<ItemGeneratorComponenet>();
            _storagePool = _world.GetPool<StorageComponent>();
            _timerPool = _world.GetPool<TimerComponenet>();
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

            ItemConverter spawnedItem = Object.Instantiate(generator.prefab);
            EcsConverter.ConvertObject(spawnedItem.gameObject, _world);

            int itemEntity = _itemFilter.GetRawEntities()[_itemFilter.GetEntitiesCount() - 1];

            ref SizeComponent size = ref _sizePool.Get(itemEntity);
            ref TransformComponenet itemTransform = ref _transformPool.Get(itemEntity);

            Vector3 spawnPos = storage.CalculateNewItemLocalPos(size.value);
            itemTransform.value.parent = storage.ItemsContainer;
            itemTransform.value.localPosition = spawnPos;
            itemTransform.value.localEulerAngles = Vector3.zero;

            Vector3 itemScale = itemTransform.value.localScale;
            itemTransform.value.localScale = Vector3.zero;
            Tween tween = itemTransform.value.DOScale(itemScale, 0.25f);

            ref ItemComponent item = ref _itemPool.Get(itemEntity);

            storage.Add(new Item(item.type, _world.PackEntity(itemEntity)));
        }
    }
}