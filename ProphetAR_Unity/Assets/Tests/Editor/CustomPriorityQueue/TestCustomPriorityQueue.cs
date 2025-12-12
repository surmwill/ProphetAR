using System;
using NUnit.Framework;
using UnityEngine;

namespace ProphetAR.Tests.CustomPriorityQueue
{
    public class TestCustomPriorityQueue
    {
        [Test]
        public void TestEnqueueDequeue()
        {
            CustomPriorityQueue<SamplePriorityQueueItem> priorityQueue = new CustomPriorityQueue<SamplePriorityQueueItem>(); 
            
            SamplePriorityQueueItem item1 = new SamplePriorityQueueItem(1);   
            SamplePriorityQueueItem item2 = new SamplePriorityQueueItem(2);
            
            Assert.IsTrue(!priorityQueue.Any());
            
            // Enqueue
            priorityQueue.Enqueue(item1);
            Assert.IsTrue(priorityQueue.Any());
            
            priorityQueue.Enqueue(item2);
            int count = 0;

            foreach (SamplePriorityQueueItem item in priorityQueue)
            {
                count++;
                Assert.IsTrue(item.Value == count);
            }

            Assert.IsTrue(count == 2);

            // Dequeue
            for (int i = 1; i <= count; i++)
            {
                SamplePriorityQueueItem item = priorityQueue.Peek();
                Assert.IsTrue(item.Value == i);
                
                item = priorityQueue.Dequeue();
                Assert.IsTrue(item.Value == i);
            }
            
            Assert.IsTrue(!priorityQueue.Any());
        }
        
        [Test]
        public void TestRemoval()
        {
            CustomPriorityQueue<SamplePriorityQueueItem> priorityQueue = new CustomPriorityQueue<SamplePriorityQueueItem>(); 
            SamplePriorityQueueItem item1 = new SamplePriorityQueueItem(1);
            SamplePriorityQueueItem item2 = new SamplePriorityQueueItem(2);
            
            Assert.IsTrue(!priorityQueue.Any());
            
            priorityQueue.Enqueue(item1);
            priorityQueue.Enqueue(item2);
            
            Assert.IsTrue(priorityQueue.Peek() == item1);
            priorityQueue.Remove(item1);
            Assert.IsTrue(priorityQueue.Peek() == item2);
            
            priorityQueue.Remove(item2);
            Assert.IsTrue(!priorityQueue.Any());
        }

        [Test]
        public void TestPriorityChange()
        {
            CustomPriorityQueue<SamplePriorityQueueItem> priorityQueue = new CustomPriorityQueue<SamplePriorityQueueItem>(); 
            
            SamplePriorityQueueItem item1 = new SamplePriorityQueueItem(1);   
            SamplePriorityQueueItem item2 = new SamplePriorityQueueItem(2);
            SamplePriorityQueueItem item3 = new SamplePriorityQueueItem(3);
            
            priorityQueue.Enqueue(item1);
            priorityQueue.Enqueue(item2);
            priorityQueue.Enqueue(item3);
            
            Assert.IsTrue(priorityQueue.Peek() == item1);
            Assert.IsTrue(item1.Priority > 1);
            
            item2.Priority = 1;
            Assert.IsTrue(priorityQueue.Peek() == item2);

            item3.Priority = 0;
            Assert.IsTrue(priorityQueue.Peek() == item3);

            // New element with the same (highest) priority gets moved to the back of the corresponding (highest) priority list
            item1.Priority = 0;
            Assert.IsTrue(priorityQueue.Peek() == item3);
            
            priorityQueue.Dequeue();
            Assert.IsTrue(priorityQueue.Peek() == item1);
            
            priorityQueue.Dequeue();
            Assert.IsTrue(priorityQueue.Peek() == item2);
        }

        [Test]
        public void TestEvents()
        {
            CustomPriorityQueue<SamplePriorityQueueItem> priorityQueue = new CustomPriorityQueue<SamplePriorityQueueItem>(); 
            SamplePriorityQueueItem item1 = new SamplePriorityQueueItem(1);
            SamplePriorityQueueItem item2 = new SamplePriorityQueueItem(2);
            
            // Setup
            int numAdds = 0;
            int numPrioChanges = 0;
            int numRemovesFromDequeue = 0;
            int numRemovesManual = 0;
            
            int prevPriority = item1.Priority;
            int newPriority = item1.Priority + 1;
            
            Action<SamplePriorityQueueItem> verifyOnAdded = itemAdded =>
            {
                Debug.Log("Added");
                numAdds++;
            };
            
            Action<SamplePriorityQueueItem, int, int> verifyOnPriorityChanged = (itemChanged, checkPrevPrio, checkNewPrio) =>
            {
                Debug.Log($"Priority changed: {prevPriority} -> {newPriority}");
                numPrioChanges++;
            };
            
            Action<SamplePriorityQueueItem, bool> verifyOnRemoved = (itemRemoved, fromDequeue) =>
            {
                Debug.Log($"Removed: {fromDequeue}");
                if (fromDequeue) 
                {
                    numRemovesFromDequeue++;
                }
                else
                {
                    numRemovesManual++;
                }
            };

            priorityQueue.OnAdded += verifyOnAdded;
            priorityQueue.OnPriorityChanged += verifyOnPriorityChanged;
            priorityQueue.OnRemoved += verifyOnRemoved;
            
            // Testing
            Assert.IsTrue(!priorityQueue.Any());
            
            // Trigger callbacks
            priorityQueue.Enqueue(item1);
            priorityQueue.Enqueue(item2);
            
            item1.Priority = newPriority;
            
            priorityQueue.Remove(item2);
            priorityQueue.Dequeue();
            
            // Ensure the callbacks actually ran
            Assert.IsTrue(numAdds == 2);
            Assert.IsTrue(numPrioChanges == 1);
            Assert.IsTrue(numRemovesManual == 1);
            Assert.IsTrue(numRemovesFromDequeue == 1);
            
            priorityQueue.OnAdded -= verifyOnAdded;
            priorityQueue.OnPriorityChanged -= verifyOnPriorityChanged;
            priorityQueue.OnRemoved -= verifyOnRemoved;
        }
        
        public class SamplePriorityQueueItem : CustomPriorityQueueItem<SamplePriorityQueueItem>
        {
            public int Value;

            public SamplePriorityQueueItem(int value)
            {
                Value = value;
            }
        }
    }
}