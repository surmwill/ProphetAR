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
    public class CustomPriorityQueue<TData> : IEnumerable<TData>
    {
        private readonly SortedDictionary<int, LinkedList<CustomPriorityQueueItem<TData>>> _data = new();

        public bool Any()
        {
            return _data.Count > 0;
        }

        public TData Peek()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }
            
            int highestPriority = _data.Keys.First();
            LinkedList<CustomPriorityQueueItem<TData>> itemQueue = _data[highestPriority];
            TData element = itemQueue.First.Value.Data;

            return element;
        }
        
        public TData Dequeue()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }

            int highestPriority = _data.Keys.First();
            
            LinkedList<CustomPriorityQueueItem<TData>> itemQueue = _data[highestPriority];
            TData element = itemQueue.First.Value.Data;
            itemQueue.RemoveFirst();

            if (itemQueue.Count == 0)
            {
                _data.Remove(highestPriority);
            }

            return element;
        }

        public void Enqueue(CustomPriorityQueueItem<TData> item)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TData>> itemQueue))
            {
                itemQueue = new LinkedList<CustomPriorityQueueItem<TData>>();
                _data[item.Priority] = itemQueue;
            }

            itemQueue.AddLast(item);
            item.OnAddedToPriorityQueue(this);
        }

        public void Remove(CustomPriorityQueueItem<TData> item)
        {
            if (!_data.TryGetValue(item.Priority, out LinkedList<CustomPriorityQueueItem<TData>> itemQueue))
            {
                throw new InvalidOperationException("There is nothing in the priority queue to remove");
            }

            LinkedListNode<CustomPriorityQueueItem<TData>> itemNode = itemQueue.Find(item);
            if (itemNode == null)
            {
                throw new InvalidOperationException("Element with given priority was not found");
            }
            
            itemQueue.Remove(itemNode);
            if (itemQueue.Count == 0)
            {
                _data.Remove(item.Priority);
                item.OnRemovedFromPriorityQueue(this);
            }
        }

        public IEnumerator<TData> GetEnumerator()
        {
            foreach (LinkedList<CustomPriorityQueueItem<TData>> itemQueue in _data.Values)
            {
                foreach (TData element in itemQueue.Select(item => item.Data))
                {
                    yield return element;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}