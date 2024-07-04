using DG.Tweening;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class ItemTransactionSolvingSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;

        private EcsPool<TransformComponenet> _transformPool;
        private EcsPool<StorageComponent> _storagePool;
        private EcsPool<SizeComponent> _sizePool;
        private EcsPool<ItemComponent> _itemPool;
        private EcsPool<ItemTransactionComponent> _transactionPool;

        private EcsWorld _world;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _filter = _world.Filter<ItemTransactionComponent>()
                .End();

            _transformPool = _world.GetPool<TransformComponenet>();
            _storagePool = _world.GetPool<StorageComponent>();
            _sizePool = _world.GetPool<SizeComponent>();
            _itemPool = _world.GetPool<ItemComponent>();
            _transactionPool = _world.GetPool<ItemTransactionComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref ItemTransactionComponent transaction = ref _transactionPool.Get(entity);

                int itemID = -1;
                int storageID = -1;

                if (transaction.item.Unpack(_world, out itemID) == false)
                    continue;

                if (transaction.storage.Unpack(_world, out storageID) == false)
                    continue;

                ref ItemComponent item = ref _itemPool.Get(itemID);
                ref StorageComponent storage = ref _storagePool.Get(storageID);

                if (storage.CanAddItem(item) == false)
                    continue;

                ref SizeComponent size = ref _sizePool.Get(itemID);
                ref TransformComponenet itemTransform = ref _transformPool.Get(itemID);

                ref TransformComponenet storageTransform = ref _transformPool.Get(storageID);

                Vector3 itemLocalPos = storage.CalculateNewItemPos(size.value);
                itemTransform.value.parent = storageTransform.value;

                storage.Add(item);

                Transform cashedItemTransform = itemTransform.value;

                var tween = cashedItemTransform.DOLocalJump(itemLocalPos, 1f, 1, 0.35f);
                var tween2 = cashedItemTransform.DOLocalRotate(Vector3.zero, 0.35f)
                    .OnComplete(() => cashedItemTransform.DOShakeScale(0.2f, 0.25f, 1));

                _transactionPool.Del(entity);
            }
        }
    }
}