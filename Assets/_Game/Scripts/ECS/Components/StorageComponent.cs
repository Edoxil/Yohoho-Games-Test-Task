using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public struct StorageComponent
    {
        [SerializeField] private int _capacity;
        private Stack<ItemComponent> _stack;

        public bool HasItems => Initialized && _stack.Count > 0;
        public bool IsFull => Initialized && _stack.Count == _capacity;

        public bool Initialized { get; private set; }

        public void Init()
        {
            if (Initialized)
                return;

            _stack = new Stack<ItemComponent>(_capacity);
            Initialized = true;
        }

        public void Add(ItemComponent item)
        {
            if (CanAddItem(item) == false)
                return;

            _stack.Push(item);
        }

        public bool CanAddItem(ItemComponent item)
        {
            if (IsFull)
                return false;

            if (HasItems && item.type != _stack.Peek().type)
                return false;

            return true;
        }

        public Vector3 CalculateNewItemPos(Vector3 size)
        {
            return Vector3.zero + (_stack.Count * size.y * Vector3.up);
        }

        public ItemComponent Remove()
        {
            return _stack.Pop();
        }
    }
}