using System;
using System.Collections.Generic;

namespace ProphetAR
{
    public abstract class CustomPriorityQueueItem<TData>
    {
        public const int DefaultPriority = 1000;
        
        public TData Data { get; }

        public event Action<CustomPriorityQueueItem<TData>, int, int> OnPriorityChanged; 
        
        public int Priority
        {
            get => _priority;
            set
            {
                if (_priority == value)
                {
                    return;
                }
                
                foreach (ICustomPriorityQueue<TData> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.Remove(this, true);
                }
                
                int prevPriority = _priority;
                _priority = value;
                
                foreach (ICustomPriorityQueue<TData> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.Enqueue(this, true);
                }
                
                OnPriorityChanged?.Invoke(this, prevPriority, _priority);
            }
        }
        
        private readonly HashSet<ICustomPriorityQueue<TData>> _partOfPriorityQueues = new();
        
        private int _priority = DefaultPriority;

        /// <summary>
        /// Called by the internals of the custom priority queue. The user shouldn't worry about calling this
        /// </summary>
        [CalledByCustomPriorityQueue]
        public void OnRemovedFromPriorityQueue(ICustomPriorityQueue<TData> removedFromQueue)
        {
            _partOfPriorityQueues.Remove(removedFromQueue);
        }

        /// <summary>
        /// Called by the internals of the custom priority queue. The user shouldn't worry about calling this
        /// </summary>
        [CalledByCustomPriorityQueue]
        public void OnAddedToPriorityQueue(ICustomPriorityQueue<TData> addToQueue)
        {
            _partOfPriorityQueues.Add(addToQueue);
        }
    }
}