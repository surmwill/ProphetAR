using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace ProphetAR.Tests.GameEvents
{
    public class TestGameEventProcessor
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
            if (gameEventProcessor.TryGetListenersForDataEvent<TestGameEventInt>(out List<IGameEventWithDataListener> listeners))
            {
                Assert.IsTrue(listeners.Count == 1, $"Only added one listener but there are {listeners.Count} present");
            }
            else
            {
                Assert.Fail("Added listener but it is not present in the listener list");
            }
            
            gameEventProcessor.RaiseEventWithData(new TestGameEventInt(1));
            Assert.IsTrue(sampleListener.LastIntData == 1);
            
            gameEventProcessor.RaiseEventWithData(new TestGameEventInt(2));
            Assert.IsTrue(sampleListener.LastIntData == 2);
            
            gameEventProcessor.RemoveListenerWithData<ITestGameEventIntListener>(sampleListener);
            if (gameEventProcessor.TryGetListenersForDataEvent<TestGameEventInt>(out listeners))
            {
                Assert.Fail($"Removed all listeners but there are still {listeners.Count} present");
            }
        }

        [Test]
        public void TestSimpleNoDataRaise()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListener sampleListener = new SampleListener();
            
            gameEventProcessor.AddListenerWithoutData<ITestGameEventNoDataListener>(sampleListener);
            if (gameEventProcessor.TryGetListenersForNonDataEvent<TestGameEventNoData>(out List<IGameEventWithoutDataListener> listeners))
            {
                Assert.IsTrue(listeners.Count == 1, $"Only added one listener but there are {listeners.Count} present");
            }
            else
            {
                Assert.Fail("Added listener but it is not present in the listener list");
            }
            
            gameEventProcessor.RaiseEventWithoutData(new TestGameEventNoData());
            Assert.IsTrue(sampleListener.NumNoDataEvents == 1);
            
            gameEventProcessor.RaiseEventWithoutData(new TestGameEventNoData());
            Assert.IsTrue(sampleListener.NumNoDataEvents == 2);
            
            gameEventProcessor.RemoveListenerWithoutData<ITestGameEventNoDataListener>(sampleListener);
            if (gameEventProcessor.TryGetListenersForNonDataEvent<TestGameEventNoData>(out listeners))
            {
                Assert.Fail($"Removed all listeners but there are still {listeners.Count} present");
            }
        }

        [Test]
        public void TestMultipleDataRaise()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListener sampleListener = new SampleListener();
            
            // Both receive the same event and therefore have the same signature. Ensure there are two different method calls
            gameEventProcessor.AddListenerWithData<ITestGameEventObjectListener, TestObject>(sampleListener);
            gameEventProcessor.AddListenerWithData<ITestGameEventObjectCopyListener, TestObject>(sampleListener);

            TestObject testObject = new TestObject(1);
            TestObject copyTestObject = new TestObject(2);
            
            gameEventProcessor.RaiseEventWithData(new TestGameEventObject(testObject));
            gameEventProcessor.RaiseEventWithData(new TestGameEventObjectCopy(copyTestObject));
            
            Assert.IsTrue(sampleListener.LastTestObjectData == testObject);
            Assert.IsTrue(sampleListener.LastTestObjectData.TestData == 1);
            Assert.IsTrue(sampleListener.NumTestObjectEvents == 1);
            
            Assert.IsTrue(sampleListener.LastTestObjectCopyData == copyTestObject);
            Assert.IsTrue(sampleListener.LastTestObjectCopyData.TestData == 2);
            Assert.IsTrue(sampleListener.NumTestObjectCopyEvents == 1);
            
            gameEventProcessor.RemoveListenerWithData<ITestGameEventObjectListener>(sampleListener);
            gameEventProcessor.RemoveListenerWithData<ITestGameEventObjectCopyListener>(sampleListener);
        }

        [Test]
        public void TestMultipleNoDataRaise()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListener sampleListener = new SampleListener();
            
            // Both receive the same event and therefore have the same signature. Ensure there are two different method calls
            gameEventProcessor.AddListenerWithoutData<ITestGameEventNoDataListener>(sampleListener);
            gameEventProcessor.AddListenerWithoutData<ITestGameEventNoDataCopyListener>(sampleListener);
            
            gameEventProcessor.RaiseEventWithoutData(new TestGameEventNoData());
            gameEventProcessor.RaiseEventWithoutData(new TestGameEventNoDataCopy());
            
            Assert.IsTrue(sampleListener.NumNoDataEvents == 1);
            Assert.IsTrue(sampleListener.NumNoDataCopyEvents == 1);
            
            gameEventProcessor.RemoveListenerWithoutData<ITestGameEventNoDataListener>(sampleListener);
            gameEventProcessor.RemoveListenerWithoutData<ITestGameEventNoDataCopyListener>(sampleListener);
        }
        
        [Test]
        public void TestMultipleListenerInstances()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListener sampleListener1 = new SampleListener();
            SampleListener sampleListener2 = new SampleListener();
            
            gameEventProcessor.AddListenerWithoutData<ITestGameEventNoDataListener>(sampleListener1);
            gameEventProcessor.AddListenerWithoutData<ITestGameEventNoDataListener>(sampleListener2);
            
            gameEventProcessor.RaiseEventWithoutData(new TestGameEventNoData());
            
            Assert.IsTrue(sampleListener1.NumNoDataEvents == 1);
            Assert.IsTrue(sampleListener2.NumNoDataEvents == 1);
            
            gameEventProcessor.RemoveListenerWithoutData<ITestGameEventNoDataListener>(sampleListener1);
            gameEventProcessor.RemoveListenerWithoutData<ITestGameEventNoDataListener>(sampleListener2);
        }
        
        [Test]
        public void TestMultipleAdd()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            SampleListener sampleListener = new SampleListener();
            
            // Both receive the same event and therefore have the same signature. Ensure there are two different method calls
            gameEventProcessor.AddListenerWithoutData<ITestGameEventNoDataListener>(sampleListener);
            gameEventProcessor.AddListenerWithoutData<ITestGameEventNoDataListener>(sampleListener);
            
            if (gameEventProcessor.TryGetListenersForNonDataEvent<TestGameEventNoData>(out List<IGameEventWithoutDataListener> listeners))
            {
                Assert.IsTrue(listeners.Count == 2, $"Added a listener twice (expecting two listeners) but there are {listeners.Count} present");
            }
            else
            {
                Assert.Fail("Added a listener twice but nothing is present in the listener list");
            }
            
            gameEventProcessor.RaiseEventWithoutData(new TestGameEventNoData());
            Assert.IsTrue(sampleListener.NumNoDataEvents == 2);
            
            gameEventProcessor.RemoveListenerWithoutData<ITestGameEventNoDataListener>(sampleListener);
            if (gameEventProcessor.TryGetListenersForNonDataEvent<TestGameEventNoData>(out listeners))
            {
                Assert.IsTrue(listeners.Count == 1);
            }
            else
            {
                Assert.Fail("Removed one of two listeners, but there is no remaining listener present");
            }
            
            
            gameEventProcessor.RemoveListenerWithoutData<ITestGameEventNoDataListener>(sampleListener);
            if (gameEventProcessor.TryGetListenersForNonDataEvent<TestGameEventNoData>(out listeners))
            {
                Assert.Fail($"Removed all listeners, but there is still {listeners.Count} present");
            }
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

            private readonly bool _printDebug;

            public SampleListener(bool printDebug = false)
            {
                _printDebug = printDebug;
            }
            
            void IGameEventWithTypedDataListener<ITestGameEventIntListener, int>.OnEvent(int data)
            {
                PrintDebug("ON INT");
                LastIntData = data;
                NumIntEvents++;
            }

            void IGameEventWithTypedDataListener<ITestGameEventObjectListener, TestObject>.OnEvent(TestObject data)
            {
                PrintDebug("ON OBJ");
                LastTestObjectData = data;
                NumTestObjectEvents++;
            }

            void IGameEventWithTypedDataListener<ITestGameEventObjectCopyListener, TestObject>.OnEvent(TestObject data)
            {
                PrintDebug("ON OBJ COPY");
                LastTestObjectCopyData = data;
                NumTestObjectCopyEvents++;
            }

            void IGameEventWithoutDataListenerExplicit<ITestGameEventNoDataListener>.OnEvent()
            {
                PrintDebug("ON NO DATA");
                NumNoDataEvents++;
            }

            void IGameEventWithoutDataListenerExplicit<ITestGameEventNoDataCopyListener>.OnEvent()
            {
                PrintDebug("ON NO DATA COPY");
                NumNoDataCopyEvents++;
            }

            private void PrintDebug(string message)
            {
                if (_printDebug)
                {
                    Debug.Log(message);
                }
            }
        }

        private class ListenerWithCallback : ITestGameEventNoDataListener
        {
            private readonly Action _callback;
            
            public ListenerWithCallback(Action callback)
            {
                _callback = callback;
            }
            
            public void OnEvent()
            {
                _callback.Invoke();
            }
        }
    }
}