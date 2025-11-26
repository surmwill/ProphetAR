using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProphetAR
{
    public class GameEventProcessor
    {
        // Maps the game event type to their corresponding list of listeners
        private readonly Dictionary<Type, List<IGameEventWithoutDataListener>> _gameEventWithoutDataListeners = new();
        private readonly Dictionary<Type, List<IGameEventWithDataListener>> _gameEventWithDataListeners = new();
        
        private static readonly Dictionary<Type, MethodInfo> DataListenerTypeToEventRaiseMethodInfo = new();
        private readonly Dictionary<IGameEventWithDataListener, Action<object>> _dataListenerToEventRaise = new(); 

        private readonly Dictionary<Type, Dictionary<long, int>> _currentEventRaiseIterations = new();
        private long _numEventRaises = 0;

        // Note: it is important this method is generic to preserve the derived interface type instead of casting it up
        public void AddListener<T>(T gameEventListener) where T : IGameEventListener
        {
            // No data
            if (gameEventListener is IGameEventWithoutDataListener gameEventWithoutDataListener)
            {
                Type gameEventType = GameEventListenerUtils.GetEventTypeForListenerType<T>();
                if (!_gameEventWithoutDataListeners.TryGetValue(gameEventType, out List<IGameEventWithoutDataListener> listenersWithoutData))
                {
                    listenersWithoutData = new List<IGameEventWithoutDataListener>();
                    _gameEventWithoutDataListeners.Add(gameEventType, listenersWithoutData);
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
            if (!_dataListenerToEventRaise.ContainsKey(gameEventWithDataListener))
            {
                Type dataListenerType = typeof(T);
                if (!DataListenerTypeToEventRaiseMethodInfo.TryGetValue(dataListenerType, out MethodInfo eventRaiseMethodInfo))
                {
                    eventRaiseMethodInfo = dataListenerType.GetMethod(IGameEventListener.OnEventMethodName);
                    DataListenerTypeToEventRaiseMethodInfo.Add(dataListenerType, eventRaiseMethodInfo);
                }
            
                _dataListenerToEventRaise.Add(gameEventWithDataListener, data => eventRaiseMethodInfo.Invoke(gameEventWithDataListener, new[] { data }));
            }
            
            // Add the listener
            Type gameEventWithDataType = GameEventListenerUtils.GetEventTypeForListenerType<T>();
            if (!_gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData))
            {
                listenersWithData = new List<IGameEventWithDataListener>();
                _gameEventWithDataListeners.Add(gameEventWithDataType, listenersWithData);
            }
            listenersWithData.Add(gameEventWithDataListener);
        }

        public void RemoveListener<T>(T gameEventListener) where T : IGameEventListener
        {
            Dictionary<long, int> iterations = null;
            int removalIndex = -1;
            
             // No data
            if (gameEventListener is IGameEventWithoutDataListener gameEventWithoutDataListener)
            {
                Type gameEventType = GameEventListenerUtils.GetEventTypeForListenerType<T>();
                if (!_gameEventWithoutDataListeners.TryGetValue(gameEventType, out List<IGameEventWithoutDataListener> listenersWithoutData)) 
                {
                    return;
                }

                // Removal might affect a current event raise
                removalIndex = listenersWithoutData.IndexOf(gameEventWithoutDataListener);
                if (removalIndex < 0)
                {
                    return;
                }
                
                if (_currentEventRaiseIterations.TryGetValue(gameEventType, out iterations))
                {
                    foreach ((long iterationKey, int iteration) in iterations)
                    {
                        if (iteration <= removalIndex)
                        {
                            iterations[iterationKey]--;
                        }
                    }
                }
                
                // Remove the listener
                listenersWithoutData.Remove(gameEventWithoutDataListener);
                if (listenersWithoutData.Count == 0)
                {
                    _gameEventWithoutDataListeners.Remove(gameEventType);
                    _currentEventRaiseIterations.Remove(gameEventType);
                }
                
                return;
            }

            // Has data
            if (!(gameEventListener is IGameEventWithDataListener gameEventWithDataListener))
            {
                Debug.LogWarning($"All listeners must derive from `{nameof(IGameEventWithoutDataListener)} or `{nameof(IGameEventWithDataListener)}`");
                return;
            }
            
            Type gameEventWithDataType = GameEventListenerUtils.GetEventTypeForListenerType<T>();
            if (!_gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData)) 
            {
                return;
            }

            // Removal might affect a current event raise
            removalIndex = listenersWithData.IndexOf(gameEventWithDataListener);
            if (removalIndex < 0)
            {
                return;
            }

            if (_currentEventRaiseIterations.TryGetValue(gameEventWithDataType, out iterations))
            {
                foreach ((long iterationKey, int iteration) in iterations)
                {
                    if (iteration <= removalIndex)
                    {
                        iterations[iterationKey]--;
                    }
                }
            }
                
            // Remove the listener
            listenersWithData.Remove(gameEventWithDataListener);
            if (listenersWithData.Count == 0)
            {
                _gameEventWithDataListeners.Remove(gameEventWithDataType);
                _currentEventRaiseIterations.Remove(gameEventWithDataType);
            }
        }

        public void RaiseEvent<T>(T gameEvent) where T : GameEvent
        {
            Dictionary<long, int> iterations = null;
            long iterationKey = 0;
            
            // No Data
            if (gameEvent is GameEventWithoutData gameEventWithoutData)
            {
                Type gameEventType = typeof(T);  
                if (!_gameEventWithoutDataListeners.TryGetValue(gameEventType, out List<IGameEventWithoutDataListener> listenersWithoutData))
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

                for (iterations[iterationKey] = 0; _currentEventRaiseIterations.ContainsKey(gameEventType) && iterations[iterationKey] < listenersWithoutData.Count; iterations[iterationKey]++)
                {
                    listenersWithoutData[iterations[iterationKey]].OnEvent();
                }
                iterations.Remove(iterationKey);

                return;
            }
            
            // Has Data
            if (!(gameEvent is GameEventWithData gameEventWithData))
            {
                Debug.LogWarning($"All game events must derive from GameEventWithoutData or GameEventWithTypedData.");
                return;
            }
            
            Type gameEventWithDataType = typeof(T);  
            if (!_gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData))
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

            for (iterations[iterationKey] = 0; _currentEventRaiseIterations.ContainsKey(gameEventWithDataType) && iterations[iterationKey] < listenersWithData.Count; iterations[iterationKey]++)
            {
                _dataListenerToEventRaise[listenersWithData[iterations[iterationKey]]].Invoke(gameEventWithData.RawData);
            }
            iterations.Remove(iterationKey);
        }
        
        public bool TryGetListenersForEvent<T>(out List<IGameEventWithoutDataListener> listeners) where T : GameEventWithoutData
        {
            Type gameEventType = typeof(T);
            if (_currentEventRaiseIterations.ContainsKey(gameEventType))
            {
                Debug.LogWarning("Possibly modification of listener list during an active event");
            }
            
            return _gameEventWithoutDataListeners.TryGetValue(gameEventType, out listeners);
        }
        
        public bool TryGetDataListenersForEvent<T>(out List<IGameEventWithDataListener> dataListeners) where T : GameEventWithData
        {
            Type gameEventWithDataType = typeof(T);
            if (_currentEventRaiseIterations.ContainsKey(gameEventWithDataType))
            {
                Debug.LogWarning("Possibly modification of listener list during an active event");
            }
            
            return _gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out dataListeners);
        }
    }
}