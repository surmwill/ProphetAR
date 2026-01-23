using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProphetAR
{
    /// <summary>
    /// Priority queue using a SortDictionary as we don't have the built-in priority queue in this c# version.
    /// Not as performant, but suitable for small collections (say < 50 elements)
    /// </summary>
    public class CustomPriorityQueue<TConcreteItem> : ICustomPriorityQueue<TConcreteItem>, IEnumerable<TConcreteItem> where TConcreteItem : CustomPriorityQueueItem<TConcreteItem>
    {
        private readonly SortedDictionary<int, LinkedList<CustomPriorityQueueItem<TConcreteItem>>> _data = new();

        public event Action<TConcreteItem> OnAdded;
        public event Action<TConcreteItem, bool> OnRemoved;
        public event Action<TConcreteItem, int, int> OnPriorityChanged;

        public bool Any()
        {
            return _data.Count > 0;
        }

        public TConcreteItem Peek()
        {
            return (TConcreteItem) ((ICustomPriorityQueue<TConcreteItem>) this).Peek();
        }
        
        CustomPriorityQueueItem<TConcreteItem> ICustomPriorityQueue<TConcreteItem>.Peek()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }
            
            int highestPriority = _data.Keys.First();
            LinkedList<CustomPriorityQueueItem<TConcreteItem>> itemQueue = _data[highestPriority];
            CustomPriorityQueueItem<TConcreteItem> item = itemQueue.First.Value;

            return item;
        }
        
        public TConcreteItem Dequeue()
        {
            TConcreteItem item = (TConcreteItem) ((ICustomPriorityQueue<TConcreteItem>) this).Dequeue();
            OnItemRemoved(item, true);
            return item;
        }
        
        CustomPriorityQueueItem<TConcreteItem> ICustomPriorityQueue<TConcreteItem>.Dequeue()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }

            int highestPriority = _data.Keys.First();
            
            LinkedList<CustomPriorityQueueItem<TConcreteItem>> itemQueue = _data[highestPriority];
            CustomPriorityQueueItem<TConcreteItem> item = itemQueue.First.Value;
            
            ((ICustomPriorityQueue<TConcreteItem>) this).Remove(item);
            return item;
        }

        public void Enqueue(TConcreteItem item)
        {
            ((ICustomPriorityQueue<TConcreteItem>) this).Enqueue(item);
            item.OnPriorityChanged += OnItemPriorityChanged;
            OnAdded?.Invoke(item);
        }
        
        void ICustomPriorityQueue<TConcreteItem>.Enqueue(CustomPriorityQueueItem<TConcreteItem> item, bool isPriorityChange)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TConcreteItem>> itemQueue))
            {
                itemQueue = new LinkedList<CustomPriorityQueueItem<TConcreteItem>>();
                _data[item.Priority] = itemQueue;
            }

            itemQueue.AddLast(item);
            if (!isPriorityChange)
            {
                item.OnAddedToPriorityQueue(this);
            }
        }

        public void Remove(TConcreteItem item)
        {
            ((ICustomPriorityQueue<TConcreteItem>) this).Remove(item);
            OnItemRemoved(item, false);
        }
        
        void ICustomPriorityQueue<TConcreteItem>.Remove(CustomPriorityQueueItem<TConcreteItem> item, bool isPriorityChange)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TConcreteItem>> itemQueue))
            {
                throw new InvalidOperationException("There is nothing in the priority queue to remove");
            }

            LinkedListNode<CustomPriorityQueueItem<TConcreteItem>> itemNode = itemQueue.Find(item);
            if (itemNode == null)
            {
                throw new InvalidOperationException("Item with given priority was not found");
            }
            
            itemQueue.Remove(itemNode);
            if (itemQueue.Count == 0)
            {
                _data.Remove(item.Priority);
            }
            
            if (!isPriorityChange)
            {
                item.OnRemovedFromPriorityQueue(this);
            }
        }

        private void OnItemRemoved(TConcreteItem item, bool fromDequeue)
        {
            item.OnPriorityChanged -= OnItemPriorityChanged; 
            OnRemoved?.Invoke(item, fromDequeue);
        }
        
        private void OnItemPriorityChanged(CustomPriorityQueueItem<TConcreteItem> item, int prevPriority, int newPriority)
        {
            OnPriorityChanged?.Invoke((TConcreteItem) item, prevPriority, newPriority);
        }

        public IEnumerator<TConcreteItem> GetEnumerator()
        {
            foreach (LinkedList<CustomPriorityQueueItem<TConcreteItem>> itemQueue in _data.Values)
            {
                foreach (CustomPriorityQueueItem<TConcreteItem> item in itemQueue)
                {
                    yield return (TConcreteItem) item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}