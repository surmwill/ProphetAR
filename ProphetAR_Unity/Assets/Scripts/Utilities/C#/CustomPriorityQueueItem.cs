using System.Collections.Generic;

namespace ProphetAR
{
    public abstract class CustomPriorityQueueItem<TData>
    {
        private const int DefaultPriority = 1000;
        
        private readonly HashSet<CustomPriorityQueue<TData>> _partOfPriorityQueues = new();
        
        public TData Data { get; }
        
        public int Priority
        {
            get => _priority;
            set
            {
                if (_priority == value)
                {
                    return;
                }
                
                foreach (CustomPriorityQueue<TData> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.Remove(this, true);
                    priorityQueue.Enqueue(this, true);
                }

                int prevPriority = _priority;
                _priority = value;
                
                foreach (CustomPriorityQueue<TData> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.OnItemNotifiedPriorityChanged(this, prevPriority, _priority);
                }
            }
        }
        
        private int _priority = DefaultPriority;

        public void NotifyNotInPriorityQueue(CustomPriorityQueue<TData> removedFromQueue)
        {
            _partOfPriorityQueues.Remove(removedFromQueue);
        }

        public void NotifyInPriorityQueue(CustomPriorityQueue<TData> addToQueue)
        {
            _partOfPriorityQueues.Add(addToQueue);
        }
    }
}