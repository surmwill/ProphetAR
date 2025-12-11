using System.Collections.Generic;

namespace ProphetAR
{
    public abstract class CustomPriorityQueueItem<TData>
    {
        private HashSet<CustomPriorityQueue<TData>> _partOfPriorityQueues = new();
        
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

                HashSet<CustomPriorityQueue<TData>> partOfQueues = _partOfPriorityQueues;
                foreach (CustomPriorityQueue<TData> priorityQueue in partOfQueues)
                {
                    priorityQueue.Remove(this);
                }

                _priority = value;
                foreach (CustomPriorityQueue<TData> priorityQueue in partOfQueues)
                {
                    priorityQueue.Enqueue(this);
                }

                _partOfPriorityQueues = partOfQueues;
            }
        }
        
        private int _priority;

        public void OnRemovedFromPriorityQueue(CustomPriorityQueue<TData> removedFromQueue)
        {
            _partOfPriorityQueues.Remove(removedFromQueue);
        }

        public void OnAddedToPriorityQueue(CustomPriorityQueue<TData> addToQueue)
        {
            _partOfPriorityQueues.Add(addToQueue);
        }
    }
}