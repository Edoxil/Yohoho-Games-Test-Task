using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public struct StorageComponent
    {
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private int _capacity;
        private Stack<Item> _stack;

        public bool Initialized { get; private set; }

        public bool HasItems => Initialized && _stack.Count > 0;
        public bool IsFull => Initialized && _stack.Count == _capacity;
        public Transform ItemsContainer => _itemsContainer;

        public void Init()
        {
            if (Initialized)
                return;

            _stack = new Stack<Item>(_capacity);
            Initialized = true;
        }

        public void Add(Item item)
        {
            if (CanAddItem(item.ItemType) == false)
                return;

            _stack.Push(item);
        }

        public Item GetItem()
        {
            if (HasItems)
                return _stack.Pop();
            else
                return null;
        }

        public bool CanAddItem(ItemType itemType)
        {
            if (IsFull)
                return false;

            if (HasItems && itemType != _stack.Peek().ItemType)
                return false;

            return true;
        }

        public Vector3 CalculateNewItemLocalPos(Vector3 size)
        {
            return (0.5f * size.y * Vector3.up) + (_stack.Count * size.y * Vector3.up);
        }
    }
}