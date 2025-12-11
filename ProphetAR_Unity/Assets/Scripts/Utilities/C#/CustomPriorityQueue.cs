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
    public class CustomPriorityQueue<TItemData> : IEnumerable<CustomPriorityQueueItem<TItemData>>
    {
        private readonly SortedDictionary<int, LinkedList<CustomPriorityQueueItem<TItemData>>> _data = new();

        public event Action<CustomPriorityQueueItem<TItemData>> OnAdded;
        public event Action<CustomPriorityQueueItem<TItemData>> OnRemoved;
        public event Action<CustomPriorityQueueItem<TItemData>, int, int> OnPriorityChanged;

        public bool Any()
        {
            return _data.Count > 0;
        }

        public CustomPriorityQueueItem<TItemData> Peek()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }
            
            int highestPriority = _data.Keys.First();
            LinkedList<CustomPriorityQueueItem<TItemData>> itemQueue = _data[highestPriority];
            CustomPriorityQueueItem<TItemData> item = itemQueue.First.Value;

            return item;
        }
        
        public CustomPriorityQueueItem<TItemData> Dequeue()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }

            int highestPriority = _data.Keys.First();
            
            LinkedList<CustomPriorityQueueItem<TItemData>> itemQueue = _data[highestPriority];
            CustomPriorityQueueItem<TItemData> item = itemQueue.First.Value;
            
            Remove(item);
            return item;
        }

        public void Enqueue(CustomPriorityQueueItem<TItemData> item, bool isPriorityChange = false)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TItemData>> itemQueue))
            {
                itemQueue = new LinkedList<CustomPriorityQueueItem<TItemData>>();
                _data[item.Priority] = itemQueue;
            }

            itemQueue.AddLast(item);
            if (!isPriorityChange)
            {
                item.NotifyInPriorityQueue(this);   
                OnAdded?.Invoke(item);
            }
        }

        public void Remove(CustomPriorityQueueItem<TItemData> item, bool isPriorityChange = false)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TItemData>> itemQueue))
            {
                throw new InvalidOperationException("There is nothing in the priority queue to remove");
            }

            LinkedListNode<CustomPriorityQueueItem<TItemData>> itemNode = itemQueue.Find(item);
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
                OnRemoved?.Invoke(item);
            }
        }

        public void OnItemNotifiedPriorityChanged(CustomPriorityQueueItem<TItemData> item, int prevPriority, int newPriority)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TItemData>> itemQueue))
            {
                throw new InvalidOperationException("The item's priority does not exist in this priority queue");
            }
            
            LinkedListNode<CustomPriorityQueueItem<TItemData>> itemNode = itemQueue.Find(item);
            if (itemNode == null)
            {
                throw new InvalidOperationException("Item was not found");
            }
            
            OnPriorityChanged?.Invoke(item, prevPriority, newPriority);
        }

        public IEnumerator<CustomPriorityQueueItem<TItemData>> GetEnumerator()
        {
            foreach (LinkedList<CustomPriorityQueueItem<TItemData>> itemQueue in _data.Values)
            {
                foreach (CustomPriorityQueueItem<TItemData> item in itemQueue)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}