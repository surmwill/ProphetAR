using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProphetAR
{
    public class GameEventProcessor
    {
        // Maps the game event type to their corresponding list of listeners
        private readonly Dictionary<Type, List<IGameEventWithoutDataListener>> GameEventWithoutDataListeners = new();
        private readonly Dictionary<Type, List<IGameEventWithDataListener>> GameEventWithDataListeners = new();
        
        private static readonly Dictionary<Type, MethodInfo> DataListenerTypeToEventRaiseMethodInfo = new();
        private readonly Dictionary<IGameEventWithDataListener, Action<object>> DataListenerToEventRaise = new(); 

        // Remember to shift these if we delete
        private LinkedListNode<CachedGameEventWithoutDataListener> _currentlyHandlingListenerWithoutDataNode;
        private LinkedListNode<CachedGameEventWithDataListener> _currentlyHandlingListenerWithDataNode;

        private readonly Dictionary<Type, Dictionary<long, int>> _currentEventRaiseIterations = new();
        private long _numEventRaises = 0;

        // Note: it is important this method is generic to preserve the specialized interface reference type instead of casting it up to a more generic base
        public void AddListener<T>(T gameEventListener) where T : IGameEventListener
        {
            // No data
            if (gameEventListener is IGameEventWithoutDataListener gameEventWithoutDataListener)
            {
                Type gameEventType = GameEventListenerUtils.GetEventTypeForListenerType<T>();
                if (!GameEventWithoutDataListeners.TryGetValue(gameEventType, out List<IGameEventWithoutDataListener> listenersWithoutData))
                {
                    listenersWithoutData = new List<IGameEventWithoutDataListener>();
                    GameEventWithoutDataListeners.Add(gameEventType, listenersWithoutData);
                }
                
                // Add the listener
                listenersWithoutData.Add(gameEventWithoutDataListener);
                return;
            }

            // Has data
            if (!(gameEventListener is IGameEventWithDataListener gameEventWithDataListener))
            {
                Debug.LogWarning($"All listeners must derive from `{nameof(IGameEventWithoutDataListener)} or `{nameof(IGameEventWithDataListener)}`");
                return;
            }
            
            // We need to figure out how to raise the event
            if (!DataListenerToEventRaise.ContainsKey(gameEventWithDataListener))
            {
                Type dataListenerType = typeof(T);
                if (!DataListenerTypeToEventRaiseMethodInfo.TryGetValue(dataListenerType, out MethodInfo eventRaiseMethodInfo))
                {
                    eventRaiseMethodInfo = dataListenerType.GetMethod(IGameEventListener.OnEventMethodName);
                    DataListenerTypeToEventRaiseMethodInfo.Add(dataListenerType, eventRaiseMethodInfo);
                }
            
                DataListenerToEventRaise.Add(gameEventWithDataListener, data => eventRaiseMethodInfo.Invoke(gameEventWithDataListener, new[] { data }));
            }
            
            // Add the listener
            Type gameEventWithDataType = GameEventListenerUtils.GetEventTypeForListenerType<T>();
            if (!GameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData))
            {
                listenersWithData = new List<IGameEventWithDataListener>();
                GameEventWithDataListeners.Add(gameEventWithDataType, listenersWithData);
            }
            listenersWithData.Add(gameEventWithDataListener);
        }
        
        public void RemoveListener

        public void RaiseEvent<T>(T gameEvent) where T : GameEvent
        {
            Dictionary<long, int> iterations = null;
            long iterationKey = 0;
            
            // No Data
            if (gameEvent is GameEventWithoutData gameEventWithoutData)
            {
                Type gameEventType = typeof(T);  
                if (!GameEventWithoutDataListeners.TryGetValue(gameEventType, out List<IGameEventWithoutDataListener> listenersWithoutData))
                {
                    Debug.LogWarning($"Raised event `{gameEventType}` with no listeners");
                    return;
                }

                if (!_currentEventRaiseIterations.TryGetValue(gameEventType, out iterations))
                {
                    iterations = new Dictionary<long, int>();
                    _currentEventRaiseIterations.Add(gameEventType, iterations);
                }

                iterationKey = _numEventRaises++;
                iterations.Add(iterationKey, 0);

                for (iterations[iterationKey] = 0; iterations[iterationKey] < listenersWithoutData.Count; iterations[iterationKey]++)
                {
                    listenersWithoutData[iterations[iterationKey]].OnEvent();
                }
            }
            
            // Has Data
            if (!(gameEvent is GameEventWithData gameEventWithData))
            {
                Debug.LogWarning($"All game events must derive from GameEventWithoutData or GameEventWithTypedData.");
                return;
            }
            
            Type gameEventWithDataType = typeof(T);  
            if (!GameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData))
            {
                Debug.LogWarning($"Raised event `{gameEventWithDataType}` with no listeners");
                return;
            }

            if (!_currentEventRaiseIterations.TryGetValue(gameEventWithDataType, out iterations))
            {
                iterations = new Dictionary<long, int>();
                _currentEventRaiseIterations.Add(gameEventWithDataType, iterations);
            }

            iterationKey = _numEventRaises++;
            iterations.Add(iterationKey, 0);

            for (iterations[iterationKey] = 0; iterations[iterationKey] < listenersWithData.Count; iterations[iterationKey]++)
            {
                DataListenerToEventRaise[listenersWithData[iterations[iterationKey]]].Invoke(gameEventWithData.RawData);
            }
        }
    }
}