using LeoEcsPhysics;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game
{
    public sealed class PlayerStorageSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EcsWorld _world;

        private EcsFilter _triggerEventFilter;
        private EcsFilter _playerFilter;
        private EcsFilter _generatorFilter;
        private EcsFilter _holderFilter;

        private EcsPool<OnTriggerStayEvent> _triggerEventPool;
        private EcsPool<TransformComponenet> _transformPool;
        private EcsPool<StorageComponent> _storagePool;
        private EcsPool<ItemTransactionComponent> _transactionPool;
        private EcsPool<ItemHolderComponent> _holderPool;

        private float _itemPickUpInterval = 0.25f;
        private float _lastPickUpAt;

        private float _itemDropInterval = 0.1f;
        private float _lastDropAt;

        public void Init(IEcsSystems systems)
        {
            _world = systems.GetWorld();

            _triggerEventFilter = _world.Filter<OnTriggerStayEvent>().End();

            _playerFilter = _world.Filter<PlayerTag>()
                .Inc<TransformComponenet>()
                .Inc<StorageComponent>()
                .End();

            _generatorFilter = _world.Filter<ItemGeneratorComponenet>()
                .Inc<TransformComponenet>()
                .Inc<StorageComponent>()
                .End();

            _holderFilter = _world.Filter<ItemHolderComponent>()
                .Inc<StorageComponent>()
                .Inc<TransformComponenet>()
                .End();

            _triggerEventPool = _world.GetPool<OnTriggerStayEvent>();
            _transformPool = _world.GetPool<TransformComponenet>();
            _storagePool = _world.GetPool<StorageComponent>();
            _transactionPool = _world.GetPool<ItemTransactionComponent>();
            _holderPool = _world.GetPool<ItemHolderComponent>();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _triggerEventFilter)
            {
                ref OnTriggerStayEvent eventData = ref _triggerEventPool.Get(entity);
                TryPickUpItem(ref eventData);
                TryDropItem(ref eventData);
            }
        }

        private bool TryPickUpItem(ref OnTriggerStayEvent eventData)
        {
            if ((Time.time - _lastPickUpAt) <= _itemPickUpInterval)
                return false;

            if (eventData.senderGameObject.TryGetComponent(out ItemGeneratorConverter itemGenerator))
            {
                int playerID = _playerFilter.GetRawEntities()[0];
                int generatorID = FindGeneratorID(itemGenerator);

                if (StorageIsFull(playerID) == false && StorageHasItems(generatorID))
                {
                    CreateItemTransaction(generatorID, playerID);
                    _lastPickUpAt = Time.time;
                    return true;
                }
            }

            return false;
        }

        private bool TryDropItem(ref OnTriggerStayEvent eventData)
        {
            if ((Time.time - _lastDropAt) <= _itemDropInterval)
                return false;

            if (eventData.senderGameObject.TryGetComponent(out ItemHolderConverter itemHolder))
            {
                int playerID = _playerFilter.GetRawEntities()[0];
                int holderID = FindHolderID(itemHolder);

                if (StorageHasItems(playerID) && StorageIsFull(holderID) == false)
                {
                    CreateItemTransaction(playerID, holderID);
                    _lastDropAt = Time.time;
                    return true;
                }
            }

            return false;
        }

        private void CreateItemTransaction(int fromStorageID, int toStorageID)
        {
            int transactionID = _world.NewEntity();
            _transactionPool.Add(transactionID);
            ref ItemTransactionComponent transaction = ref _transactionPool.Get(transactionID);

            transaction.fromStorage = _world.PackEntity(fromStorageID);
            transaction.toStorage = _world.PackEntity(toStorageID);
        }

        private bool StorageIsFull(int storageID)
        {
            ref StorageComponent storage = ref _storagePool.Get(storageID);
            return storage.IsFull;
        }

        private bool StorageHasItems(int storageID)
        {
            ref StorageComponent storage = ref _storagePool.Get(storageID);
            return storage.HasItems;
        }

        private int FindGeneratorID(ItemGeneratorConverter itemGenerator)
        {
            foreach (var genID in _generatorFilter)
            {
                ref TransformComponenet genTransform = ref _transformPool.Get(genID);

                if (genTransform.value == itemGenerator.transform)
                    return genID;
            }

            return -1;
        }

        private int FindHolderID(ItemHolderConverter itemHolder)
        {
            foreach (var holderID in _holderFilter)
            {
                ref TransformComponenet holderTransform = ref _transformPool.Get(holderID);

                if (holderTransform.value == itemHolder.transform)
                    return holderID;
            }

            return -1;
        }
    }
}