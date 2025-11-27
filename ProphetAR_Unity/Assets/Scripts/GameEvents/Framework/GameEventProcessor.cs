using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ProphetAR
{
    public class GameEventProcessor
    {
        // Maps the game event type to their corresponding list of listeners
        private readonly Dictionary<Type, List<IGameEventWithoutDataListener>> _gameEventWithoutDataListeners = new();
        private readonly Dictionary<Type, List<IGameEventWithDataListener>> _gameEventWithDataListeners = new();
        
        // For each type of game event listener that takes data, store the method that receives that data
        private static readonly Dictionary<Type, MethodInfo> DataListenerTypeToEventRaiseMethodInfo = new();
        
        // For each concrete game event listener that takes data, store how we send various data to that listener
        private readonly Dictionary<IGameEventWithDataListener, Dictionary<Type, Action<object>>> _dataListenerToEventRaises = new();
        
        private readonly Dictionary<Type, Dictionary<long, int>> _currentEventRaiseIterations = new();
        private long _numEventRaises = 0;
        
        public void AddListenerWithoutData<TListener>(IGameEventWithoutDataListener listenerInstance) where TListener : IGameEventWithoutDataListener
        {
            Type gameEventType = GameEventListenerUtils.GetEventTypeForListenerType<TListener>();
            if (!_gameEventWithoutDataListeners.TryGetValue(gameEventType, out List<IGameEventWithoutDataListener> listenersWithoutData))
            {
                listenersWithoutData = new List<IGameEventWithoutDataListener>();
                _gameEventWithoutDataListeners.Add(gameEventType, listenersWithoutData);
            }
                
            // Add the listener
            listenersWithoutData.Add(listenerInstance);
        }
        
        public void AddListenerWithData<TListener, TListenerData>(IGameEventWithDataListener listenerInstance) where TListener : IGameEventWithDataListener
        {
            // We need to figure out how to raise the event
            if (!_dataListenerToEventRaises.TryGetValue(listenerInstance, out Dictionary<Type, Action<object>> eventRaises))
            {
                eventRaises = new Dictionary<Type, Action<object>>();
                _dataListenerToEventRaises.Add(listenerInstance, eventRaises);
            }
            
            Type listenerType = typeof(TListener);
            Type gameEventWithDataType = GameEventListenerUtils.GetEventTypeForListenerType<TListener>();
            
            if (!eventRaises.ContainsKey(gameEventWithDataType))
            {
                if (!DataListenerTypeToEventRaiseMethodInfo.TryGetValue(listenerType, out MethodInfo eventRaiseMethodInfo))
                {
                    // Every listener that receives data should directly derive from IGameEventWithTypedDataListener
                    eventRaiseMethodInfo = GetDirectParentInterface(listenerType).GetMethod(IGameEventListener.OnEventMethodName);
                    DataListenerTypeToEventRaiseMethodInfo.Add(listenerType, eventRaiseMethodInfo);
                }
                
                Type delegateType = typeof(Action<TListenerData>);
                Delegate closedDelegate = eventRaiseMethodInfo.CreateDelegate(delegateType, listenerInstance);
                
                eventRaises.Add(gameEventWithDataType, data => ((Action<TListenerData>) closedDelegate).Invoke((TListenerData) data));
            }
            
            // Add the listener
            if (!_gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData))
            {
                listenersWithData = new List<IGameEventWithDataListener>();
                _gameEventWithDataListeners.Add(gameEventWithDataType, listenersWithData);
            }
            listenersWithData.Add(listenerInstance);
        }
        
        public void RemoveListenerWithoutData<TListener>(IGameEventWithoutDataListener listenerInstance) where TListener : IGameEventWithoutDataListener
        {
            Type gameEventType = GameEventListenerUtils.GetEventTypeForListenerType<TListener>();
            if (!_gameEventWithoutDataListeners.TryGetValue(gameEventType, out List<IGameEventWithoutDataListener> listenersWithoutData)) 
            {
                return;
            }

            // Removal might affect a current event raise
            int removalIndex = listenersWithoutData.IndexOf(listenerInstance);
            if (removalIndex < 0)
            {
                return;
            }
            
            if (_currentEventRaiseIterations.TryGetValue(gameEventType, out Dictionary<long, int> iterations))
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
            listenersWithoutData.Remove(listenerInstance);
            if (listenersWithoutData.Count == 0)
            {
                _gameEventWithoutDataListeners.Remove(gameEventType);
                _currentEventRaiseIterations.Remove(gameEventType);
            }
        }
        
        public void RemoveListenerWithData<TListener>(IGameEventWithDataListener listenerInstance) where TListener : IGameEventWithDataListener
        {
            Type gameEventWithDataType = GameEventListenerUtils.GetEventTypeForListenerType<TListener>();
            if (!_gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData)) 
            {
                return;
            }

            // Removal might affect a current event raise
            int removalIndex = listenersWithData.IndexOf(listenerInstance);
            if (removalIndex < 0)
            {
                return;
            }

            if (_currentEventRaiseIterations.TryGetValue(gameEventWithDataType, out Dictionary<long, int> iterations))
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
            listenersWithData.Remove(listenerInstance);
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
                _dataListenerToEventRaises[listenersWithData[iterations[iterationKey]]][gameEventWithDataType].Invoke(gameEventWithData.RawData);
            }
            iterations.Remove(iterationKey);
        }
        
        public bool TryGetListenersWithoutData<TEventWithoutData>(out List<IGameEventWithoutDataListener> listeners) where TEventWithoutData : GameEventWithoutData
        {
            Type gameEventType = typeof(TEventWithoutData);
            if (_currentEventRaiseIterations.ContainsKey(gameEventType))
            {
                Debug.LogWarning("Possibly modification of listener list during an active event");
            }
            
            return _gameEventWithoutDataListeners.TryGetValue(gameEventType, out listeners);
        }
        
        public bool TryGetDataListenersWithData<TEventWithData>(out List<IGameEventWithDataListener> dataListeners) where TEventWithData : GameEventWithData
        {
            Type gameEventWithDataType = typeof(TEventWithData);
            if (_currentEventRaiseIterations.ContainsKey(gameEventWithDataType))
            {
                Debug.LogWarning("Possibly modification of listener list during an active event");
            }
            
            return _gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out dataListeners);
        }

        private static Type GetDirectParentInterface(Type t)
        {
            Type[] allInterfaces = t.GetInterfaces();
            if (allInterfaces.Length == 0)
            {
                return null;
            }

            IEnumerable<Type> allAncestors = allInterfaces.SelectMany(i => i.GetInterfaces());
            return allInterfaces.Except(allAncestors).FirstOrDefault();
        }
    }
}