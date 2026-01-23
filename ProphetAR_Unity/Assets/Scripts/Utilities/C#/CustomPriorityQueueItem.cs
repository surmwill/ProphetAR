using System;
using System.Collections.Generic;

namespace ProphetAR
{
    public abstract class CustomPriorityQueueItem<TConcreteItem> where TConcreteItem : CustomPriorityQueueItem<TConcreteItem>
    {
        public event Action<CustomPriorityQueueItem<TConcreteItem>, int, int> OnPriorityChanged; 
        
        public int Priority
        {
            get => PriorityOrDefault;
            set
            {
                if (PriorityOrDefault == value)
                {
                    return;
                }
                
                foreach (ICustomPriorityQueue<TConcreteItem> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.Remove(this, true);
                }
                
                int prevPriority = PriorityOrDefault;
                PriorityOrDefault = value;
                
                foreach (ICustomPriorityQueue<TConcreteItem> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.Enqueue(this, true);
                }
                
                OnPriorityChanged?.Invoke(this, prevPriority, PriorityOrDefault);
            }
        }

        public TConcreteItem Data => _data ??= (TConcreteItem) this;
        
        protected virtual int DefaultPriority { get; } = 1000;

        private int PriorityOrDefault
        {
            get => _priorityBacking ??= DefaultPriority;
            set => _priorityBacking = value;
        }
        
        private readonly HashSet<ICustomPriorityQueue<TConcreteItem>> _partOfPriorityQueues = new();
        
        private TConcreteItem _data;
        
        private int? _priorityBacking;

        /// <summary>
        /// Called by the internals of the custom priority queue. The user shouldn't worry about calling this
        /// </summary>
        [CalledByCustomPriorityQueue]
        public void OnRemovedFromPriorityQueue(ICustomPriorityQueue<TConcreteItem> removedFromQueue)
        {
            _partOfPriorityQueues.Remove(removedFromQueue);
        }

        /// <summary>
        /// Called by the internals of the custom priority queue. The user shouldn't worry about calling this
        /// </summary>
        [CalledByCustomPriorityQueue]
        public void OnAddedToPriorityQueue(ICustomPriorityQueue<TConcreteItem> addToQueue)
        {
            _partOfPriorityQueues.Add(addToQueue);
        }
    }
}