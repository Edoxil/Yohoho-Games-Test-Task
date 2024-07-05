using DG.Tweening;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class ItemTransactionHandleSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsFilter _filter;

        private EcsPool<TransformComponenet> _transformPool;
        private EcsPool<StorageComponent> _storagePool;
        private EcsPool<SizeComponent> _sizePool;
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
            _transactionPool = _world.GetPool<ItemTransactionComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter)
            {
                ref ItemTransactionComponent transaction = ref _transactionPool.Get(entity);

                int fromStorageID = -1;
                int toStorageID = -1;

                if (transaction.fromStorage.Unpack(_world, out fromStorageID) == false)
                    continue;

                if (transaction.toStorage.Unpack(_world, out toStorageID) == false)
                    continue;
                
                ref StorageComponent fromStorage = ref _storagePool.Get(fromStorageID);
                ref StorageComponent toStorage = ref _storagePool.Get(toStorageID);

                Item item = fromStorage.GetItem();

                int itemID = -1;

                if (item.PackedEntity.Unpack(_world, out itemID) == false)
                    continue;

                ref SizeComponent size = ref _sizePool.Get(itemID);
                ref TransformComponenet itemTransform = ref _transformPool.Get(itemID);

                Vector3 itemLocalPos = toStorage.CalculateNewItemLocalPos(size.value);
                itemTransform.value.parent = toStorage.ItemsContainer;

                toStorage.Add(item);

                Transform cashedItemTransform = itemTransform.value;

                Tween moveTween = cashedItemTransform.DOLocalJump(itemLocalPos, 1f, 1, 0.35f);
                Tween rotateTween = cashedItemTransform.DOLocalRotate(Vector3.zero, 0.35f)
                    .OnComplete(() => cashedItemTransform.DOShakeScale(0.2f, 0.25f, 1));

                _transactionPool.Del(entity);
            }
        }
    }
}