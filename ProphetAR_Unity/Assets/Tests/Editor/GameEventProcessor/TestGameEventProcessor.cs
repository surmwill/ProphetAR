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
        public void TestSimpleDataRaise()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListener sampleListener = new SampleListener();
            
            gameEventProcessor.AddListenerWithData<ITestGameEventIntListener, int>(sampleListener);
            
            gameEventProcessor.RaiseEventWithData(new TestGameEventInt(5));
            Debug.Log(sampleListener.LastIntData);
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