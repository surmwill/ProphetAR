using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    public class TestGameEvents : MonoBehaviour
    {
        [SerializeField]
        private Button _eventAButton = null;

        [SerializeField]
        private Button _eventBButton = null;

        private void Start()
        {
            GameEventProcessor gameEventProcessor = new GameEventProcessor();
            ConcreteListener concreteListener = new ConcreteListener();
            
            gameEventProcessor.AddListenerWithoutData<IEventAListener>(concreteListener);
            gameEventProcessor.AddListenerWithoutData<IEventBListener>(concreteListener);
            
            gameEventProcessor.AddListenerWithData<IEventAWithDataListener, int>(concreteListener);
            gameEventProcessor.AddListenerWithData<IEventBWithDataListener, int>(concreteListener);
            
            _eventAButton.onClick.AddListener(() =>
            {
                gameEventProcessor.RaiseEventWithoutData(new EventA());
                gameEventProcessor.RaiseEventWithData(new EventAWithData(1));
            });
            
            _eventBButton.onClick.AddListener(() =>
            {
                gameEventProcessor.RaiseEventWithoutData(new EventB());
                gameEventProcessor.RaiseEventWithData(new EventBWithData(2));
            });
        }

        private class ConcreteListener : IEventAListener, IEventBListener, IEventAWithDataListener, IEventBWithDataListener
        {
            void IGameEventWithoutDataListenerExplicit<IEventAListener>.OnEvent()
            {
                Debug.Log("ON A");
            }

            void IGameEventWithoutDataListenerExplicit<IEventBListener>.OnEvent()
            {
                Debug.Log("ON B");
            }

            void IGameEventWithTypedDataListener<IEventAWithDataListener, int>.OnEvent(int data)
            {
                Debug.Log($"ON A WITH DATA {data}");
            }

            void IGameEventWithTypedDataListener<IEventBWithDataListener, int>.OnEvent(int data)
            {
                Debug.Log($"ON B WITH DATA {data}");
            }
        }

        private class EventA : GameEventWithoutData
        {
            
        }

        private class EventB : GameEventWithoutData
        {
            
        }
        
        private class EventAWithData : GameEventWithTypedData<int>
        {
            public EventAWithData(int data) : base(data)
            {
            }
        }

        private class EventBWithData : GameEventWithTypedData<int>
        {
            public EventBWithData(int data) : base(data)
            {
            }
        }
        
        [ListensToGameEventType(typeof(EventA))]
        private interface IEventAListener : IGameEventWithoutDataListenerExplicit<IEventAListener>
        {
            
        }
        
        [ListensToGameEventType(typeof(EventB))]
        private interface IEventBListener : IGameEventWithoutDataListenerExplicit<IEventBListener>
        {
            
        }

        [ListensToGameEventType(typeof(EventAWithData))]
        private interface IEventAWithDataListener : IGameEventWithTypedDataListener<IEventAWithDataListener, int>
        {
            
        }
        
        [ListensToGameEventType(typeof(EventBWithData))]
        private interface IEventBWithDataListener : IGameEventWithTypedDataListener<IEventBWithDataListener, int>
        {
            
        }
    }
}
