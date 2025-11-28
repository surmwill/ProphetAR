using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace ProphetAR.Tests.GameEvents
{
    public partial class TestGameEventProcessor
    {
        // Test basic raise
        
        // Test multiple raises 
        
        // Test raises, add, and the added gets raised too
        
        // Test raise, move to next, remove first, and we should exit
        
        // Test multiple objects with same listeners
        
        // Test same 
        
        // Add anti-stripping
        
        // Test multiple adds and removes
        
        [Test]
        public void TestDataRaise()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListener sampleListener = new SampleListener();
            
            gameEventProcessor.AddListenerWithData<ITestGameEventIntListener, int>(sampleListener);
            if (gameEventProcessor.TryGetListenersForDataEvent<TestGameEventInt>(out List<IGameEventWithDataListener> listeners))
            {
                Assert.IsTrue(listeners.Count == 1, $"Only added one listener but there are {listeners.Count} present");
            }
            else
            {
                Assert.Fail("Added listener but it is not present in the listener list");
            }
            
            gameEventProcessor.RaiseEventWithData(new TestGameEventInt(5));
            Assert.IsTrue(sampleListener.LastIntData == 5);
            
            gameEventProcessor.RaiseEventWithData(new TestGameEventInt(6));
            Assert.IsTrue(sampleListener.LastIntData == 6);
            
            gameEventProcessor.RemoveListenerWithData<ITestGameEventIntListener>(sampleListener);
            if (gameEventProcessor.TryGetListenersForDataEvent<TestGameEventInt>(out listeners))
            {
                Assert.Fail($"Removed all listeners but there are still {listeners.Count} present");
            }
        }

        [Test]
        public void TestSimpleNoDataRaise()
        {
        
        }

        private class SampleListener : 
            ITestGameEventIntListener, 
            ITestGameEventObjectListener, 
            ITestGameEventObjectCopyListener,
            ITestGameEventNoDataListener,
            ITestGameEventNoDataCopyListener
        {
            public int LastIntData { get; private set; }
            
            public TestObject LastTestObjectData { get; private set; }
            
            public TestObject LastTestObjectCopyData { get; private set; }
            
            public int NumIntEvents { get; private set; }
            
            public int NumTestObjectEvents { get; private set; }
            
            public int NumTestObjectCopyEvents { get; private set; }
            
            public int NumNoDataEvents { get; private set; }
            
            public int NumNoDataCopyEvents { get; private set; }
            
            void IGameEventWithTypedDataListener<ITestGameEventIntListener, int>.OnEvent(int data)
            {
                LastIntData = data;
                NumIntEvents++;
            }

            void IGameEventWithTypedDataListener<ITestGameEventObjectListener, TestObject>.OnEvent(TestObject data)
            {
                LastTestObjectData = data;
                NumTestObjectEvents++;
            }

            void IGameEventWithTypedDataListener<ITestGameEventObjectCopyListener, TestObject>.OnEvent(TestObject data)
            {
                LastTestObjectCopyData = data;
                NumTestObjectCopyEvents++;
            }

            void IGameEventWithoutDataListenerExplicit<ITestGameEventNoDataListener>.OnEvent()
            {
                NumNoDataEvents++;
            }

            void IGameEventWithoutDataListenerExplicit<ITestGameEventNoDataCopyListener>.OnEvent()
            {
                NumNoDataCopyEvents++;
            }
        }
    }
}