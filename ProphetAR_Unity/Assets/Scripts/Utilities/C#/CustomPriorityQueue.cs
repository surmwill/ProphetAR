using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProphetAR
{
    /// <summary>
    /// Priority queue using a SortDictionary as we don't have the built-in priority queue in this c# version.
    /// Not as performant, but suitable for small collections (< 50 elements)
    /// </summary>
    public class CustomPriorityQueue<TItem> : ICustomPriorityQueue<TItem>, IEnumerable<TItem> where TItem : CustomPriorityQueueItem<TItem>
    {
        private readonly SortedDictionary<int, LinkedList<CustomPriorityQueueItem<TItem>>> _data = new();

        public event Action<TItem> OnAdded;
        public event Action<TItem> OnRemoved;
        public event Action<TItem, int, int> OnPriorityChanged;

        public bool Any()
        {
            return _data.Count > 0;
        }

        public TItem Peek()
        {
            return (TItem) ((ICustomPriorityQueue<TItem>) this).Peek();
        }
        
        CustomPriorityQueueItem<TItem> ICustomPriorityQueue<TItem>.Peek()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }
            
            int highestPriority = _data.Keys.First();
            LinkedList<CustomPriorityQueueItem<TItem>> itemQueue = _data[highestPriority];
            CustomPriorityQueueItem<TItem> item = itemQueue.First.Value;

            return item;
        }
        
        public TItem Dequeue()
        {
            return (TItem) ((ICustomPriorityQueue<TItem>) this).Dequeue();
        }
        
        CustomPriorityQueueItem<TItem> ICustomPriorityQueue<TItem>.Dequeue()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }

            int highestPriority = _data.Keys.First();
            
            LinkedList<CustomPriorityQueueItem<TItem>> itemQueue = _data[highestPriority];
            CustomPriorityQueueItem<TItem> item = itemQueue.First.Value;
            
            ((ICustomPriorityQueue<TItem>) this).Remove(item);
            return item;
        }

        public void Enqueue(TItem item, bool isPriorityChange = false)
        {
            ((ICustomPriorityQueue<TItem>) this).Enqueue(item, isPriorityChange);
            OnAdded?.Invoke(item);
        }
        
        void ICustomPriorityQueue<TItem>.Enqueue(CustomPriorityQueueItem<TItem> item, bool isPriorityChange)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TItem>> itemQueue))
            {
                itemQueue = new LinkedList<CustomPriorityQueueItem<TItem>>();
                _data[item.Priority] = itemQueue;
            }

            itemQueue.AddLast(item);
            if (!isPriorityChange)
            {
                item.NotifyInPriorityQueue(this);   
            }
        }

        public void Remove(TItem item, bool isPriorityChange = false)
        {
            ((ICustomPriorityQueue<TItem>) this).Remove(item, isPriorityChange);
            OnRemoved?.Invoke(item);
        }
        
        void ICustomPriorityQueue<TItem>.Remove(CustomPriorityQueueItem<TItem> item, bool isPriorityChange)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TItem>> itemQueue))
            {
                throw new InvalidOperationException("There is nothing in the priority queue to remove");
            }

            LinkedListNode<CustomPriorityQueueItem<TItem>> itemNode = itemQueue.Find(item);
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
                item.NotifyNotInPriorityQueue(this);
            }
        }

        public void OnItemNotifiedPriorityChanged(TItem item, int prevPriority, int newPriority)
        {
            ((ICustomPriorityQueue<TItem>) this).OnItemNotifiedPriorityChanged(item, prevPriority, newPriority);
            OnPriorityChanged?.Invoke(item, prevPriority, newPriority);
        }

        void ICustomPriorityQueue<TItem>.OnItemNotifiedPriorityChanged(CustomPriorityQueueItem<TItem> item, int prevPriority, int newPriority)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TItem>> itemQueue))
            {
                throw new InvalidOperationException("The item's priority does not exist in this priority queue");
            }
            
            LinkedListNode<CustomPriorityQueueItem<TItem>> itemNode = itemQueue.Find(item);
            if (itemNode == null)
            {
                throw new InvalidOperationException("Item was not found");
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (LinkedList<CustomPriorityQueueItem<TItem>> itemQueue in _data.Values)
            {
                foreach (CustomPriorityQueueItem<TItem> item in itemQueue)
                {
                    yield return (TItem) item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}