using System.Collections.Generic;

namespace ProphetAR
{
    public interface ICustomPriorityQueue<TItemData>
    {
        public CustomPriorityQueueItem<TItemData> Peek();
        
        public bool Any();
        
        public CustomPriorityQueueItem<TItemData> Dequeue();
        
        public void Enqueue(CustomPriorityQueueItem<TItemData> item, bool isPriorityChange = false);

        public void Remove(CustomPriorityQueueItem<TItemData> item, bool isPriorityChange = false);
    }
}