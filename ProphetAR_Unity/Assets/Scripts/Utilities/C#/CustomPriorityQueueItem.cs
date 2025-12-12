using System.Collections.Generic;

namespace ProphetAR
{
    public abstract class CustomPriorityQueueItem<TData>
    {
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
                
                foreach (ICustomPriorityQueue<TData> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.Remove(this, true);
                    priorityQueue.Enqueue(this, true);
                }

                int prevPriority = _priority;
                _priority = value;
                
                foreach (ICustomPriorityQueue<TData> priorityQueue in _partOfPriorityQueues)
                {
                    priorityQueue.OnItemNotifiedPriorityChanged(this, prevPriority, _priority);
                }
            }
        }
        
        private const int DefaultPriority = 1000;
        
        private readonly HashSet<ICustomPriorityQueue<TData>> _partOfPriorityQueues = new();
        
        private int _priority = DefaultPriority;

        public void NotifyNotInPriorityQueue(ICustomPriorityQueue<TData> removedFromQueue)
        {
            _partOfPriorityQueues.Remove(removedFromQueue);
        }

        public void NotifyInPriorityQueue(ICustomPriorityQueue<TData> addToQueue)
        {
            _partOfPriorityQueues.Add(addToQueue);
        }
    }
}