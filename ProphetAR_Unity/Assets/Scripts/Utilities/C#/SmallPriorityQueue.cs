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
    public class SmallPriorityQueue<TElement, TPriority> : IEnumerable<TElement> where TPriority : IComparable<TPriority>
    {
        private readonly SortedDictionary<TPriority, LinkedList<TElement>> _data = new();

        public bool Any()
        {
            return _data.Count > 0;
        }

        public int Count()
        {
            int count = 0;
            foreach (TElement element in this)
            {
                count++;
            }
            return count;
        }

        public TElement Peek()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }
            
            TPriority highestPriority = _data.Keys.First();
            LinkedList<TElement> elementQueue = _data[highestPriority];
            TElement element = elementQueue.First.Value;

            return element;
        }
        
        public TElement Dequeue()
        {
            if (_data.Count == 0)
            {
                throw new InvalidOperationException("Empty queue");
            }

            TPriority highestPriority = _data.Keys.First();
            
            LinkedList<TElement> elementQueue = _data[highestPriority];
            TElement element = elementQueue.First.Value;
            elementQueue.RemoveFirst();

            if (elementQueue.Count == 0)
            {
                _data.Remove(highestPriority);
            }

            return element;
        }

        public void Enqueue(TElement element, TPriority priority)
        {
            if (!_data.TryGetValue(priority, out LinkedList<TElement> elementQueue))
            {
                elementQueue = new LinkedList<TElement>();
                _data[priority] = elementQueue;
            }

            elementQueue.AddLast(element);
        }

        public void Remove(TElement element, TPriority priority)
        {
            if (!_data.TryGetValue(priority, out LinkedList<TElement> elementQueue))
            {
                throw new InvalidOperationException("There is nothing in the priority queue to remove");
            }

            LinkedListNode<TElement> elementNode = elementQueue.Find(element);
            if (elementNode == null)
            {
                throw new InvalidOperationException("Element with given priority was not found");
            }
            
            elementQueue.Remove(elementNode);
            if (elementQueue.Count == 0)
            {
                _data.Remove(priority);
            }
        }

        public void ChangePriority(TElement element, TPriority prevPriority, TPriority newPriority)
        {
            Remove(element, prevPriority);
            Enqueue(element, newPriority);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            foreach (LinkedList<TElement> elementQueue in _data.Values)
            {
                foreach (TElement element in elementQueue)
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