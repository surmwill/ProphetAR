using System;
using System.Collections.Generic;

namespace ProphetAR
{
    public abstract class CustomPriorityQueueItem<TData>
    {
        public TData Data { get; }

        public event Action<CustomPriorityQueueItem<TData>, int, int> OnPriorityChanged; 
        
        public int Priority
        {
            get => PriorityOrDefault;
            set
            {
                if (PriorityOrDefault == value)
                {
                    return;
                }
                
                foreach (ICustomPriorityQueue<TData> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.Remove(this, true);
                }
                
                int prevPriority = PriorityOrDefault;
                PriorityOrDefault = value;
                
                foreach (ICustomPriorityQueue<TData> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.Enqueue(this, true);
                }
                
                OnPriorityChanged?.Invoke(this, prevPriority, PriorityOrDefault);
            }
        }
        
        protected virtual int DefaultPriority { get; } = 1000;

        private int PriorityOrDefault
        {
            get => _priorityBacking ??= DefaultPriority;
            set => _priorityBacking = value;
        }
        
        private int? _priorityBacking;
        
        private readonly HashSet<ICustomPriorityQueue<TData>> _partOfPriorityQueues = new();

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