
namespace ProphetAR
{
    public interface ICustomPriorityQueue<TConcreteItem> where TConcreteItem : CustomPriorityQueueItem<TConcreteItem>
    {
        public CustomPriorityQueueItem<TConcreteItem> Peek();
        
        public bool Any();
        
        public CustomPriorityQueueItem<TConcreteItem> Dequeue();
        
        public void Enqueue(CustomPriorityQueueItem<TConcreteItem> item, bool isPriorityChange = false);

        public void Remove(CustomPriorityQueueItem<TConcreteItem> item, bool isPriorityChange = false);
    }
}