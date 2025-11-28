using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ProphetAR
{
    public class GameEventProcessor
    {
        // These two interfaces are what each listener should derive from
        private const string InterfaceNameTypedGameEventListener = "IGameEventWithTypedDataListener";
        private const string InterfaceNameGameEventWithoutDataListener = "IGameEventWithoutDataListenerExplicit";
        
        // Maps the game event type to their corresponding list of listeners
        private readonly Dictionary<Type, List<IGameEventWithoutDataListener>> _gameEventWithoutDataListeners = new();
        private readonly Dictionary<Type, List<IGameEventWithDataListener>> _gameEventWithDataListeners = new();
        
        // For each type of listener that takes data, store the method that receives that data
        private static readonly Dictionary<Type, MethodInfo> DataListenerTypeToEventRaiseMethodInfo = new();
        
        // For each concrete listener instance, store the event raise we need to call for each game event type
        private readonly Dictionary<IGameEventWithDataListener, Dictionary<Type, Action<object>>> _dataListenerToEventRaises = new();
        private readonly Dictionary<IGameEventWithoutDataListener, Dictionary<Type, Action>> _noDataListenerToEventRaises = new();
        
        // Track the number of listener instances
        private readonly Dictionary<IGameEventListener, int> _listenerInstances = new();
        
        private readonly Dictionary<Type, Dictionary<long, int>> _currentEventRaiseIterations = new();
        private long _numEventRaises = 0;
        
        public void AddListenerWithoutData<TListener>(IGameEventWithoutDataListener listenerInstance) where TListener : IGameEventWithoutDataListener
        {
            // We need to figure out how to raise the event
            if (!_noDataListenerToEventRaises.TryGetValue(listenerInstance, out Dictionary<Type, Action> eventRaises))
            {
                eventRaises = new Dictionary<Type, Action>();
                _noDataListenerToEventRaises.Add(listenerInstance, eventRaises);
            }
            
            Type listenerType = typeof(TListener);
            Type gameEventWithoutDataType = GameEventListenerUtils.GetEventTypeForListenerType<TListener>();
            
            if (!eventRaises.ContainsKey(gameEventWithoutDataType))
            {
                if (!DataListenerTypeToEventRaiseMethodInfo.TryGetValue(listenerType, out MethodInfo eventRaiseMethodInfo))
                {
                    // Every listener that receives data should directly derive from IGameEventWithTypedDataListener
                    eventRaiseMethodInfo = GetImplementedInterfaceType(listenerType, InterfaceNameGameEventWithoutDataListener).GetMethod(IGameEventListener.OnEventMethodName);
                    DataListenerTypeToEventRaiseMethodInfo.Add(listenerType, eventRaiseMethodInfo);
                }
                
                Type delegateType = typeof(Action);
                Delegate closedDelegate = eventRaiseMethodInfo.CreateDelegate(delegateType, listenerInstance);
                
                eventRaises.Add(gameEventWithoutDataType, () => ((Action) closedDelegate).Invoke());
            }
            
            // Add the listener
            if (!_gameEventWithoutDataListeners.TryGetValue(gameEventWithoutDataType, out List<IGameEventWithoutDataListener> listenersWithoutData))
            {
                listenersWithoutData = new List<IGameEventWithoutDataListener>();
                _gameEventWithoutDataListeners.Add(gameEventWithoutDataType, listenersWithoutData);
            }
            listenersWithoutData.Add(listenerInstance);

            if (!_listenerInstances.ContainsKey(listenerInstance))
            {
                _listenerInstances[listenerInstance] = 1;
            }
            else
            {
                _listenerInstances[listenerInstance]++;
            }
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
                    eventRaiseMethodInfo = GetImplementedInterfaceType(listenerType, InterfaceNameTypedGameEventListener).GetMethod(IGameEventListener.OnEventMethodName);
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

            if (!_listenerInstances.ContainsKey(listenerInstance))
            {
                _listenerInstances[listenerInstance] = 1;
            }
            else
            {
                _listenerInstances[listenerInstance]++;
            }
        }
        
        public void RemoveListenerWithoutData<TListener>(IGameEventWithoutDataListener listenerInstance) where TListener : IGameEventWithoutDataListener
        {
            Type gameEventType = GameEventListenerUtils.GetEventTypeForListenerType<TListener>();
            if (!_gameEventWithoutDataListeners.TryGetValue(gameEventType, out List<IGameEventWithoutDataListener> listenersWithoutData)) 
            {
                return;
            }
            
            int removalIndex = listenersWithoutData.IndexOf(listenerInstance);
            if (removalIndex < 0)
            {
                return;
            }
            
            // Remove the listener
            listenersWithoutData.RemoveAt(removalIndex);
            if (listenersWithoutData.Count == 0)
            {
                _gameEventWithoutDataListeners.Remove(gameEventType);
                _currentEventRaiseIterations.Remove(gameEventType);
            }

            if (--_listenerInstances[listenerInstance] == 0)
            {
                _listenerInstances.Remove(listenerInstance);
                _noDataListenerToEventRaises.Remove(listenerInstance);
            }
            
            // Removal might affect a current event raise
            UpdateIterationsWithRemovedListener(gameEventType, removalIndex);
        }
        
        public void RemoveListenerWithData<TListener>(IGameEventWithDataListener listenerInstance) where TListener : IGameEventWithDataListener
        {
            Type gameEventWithDataType = GameEventListenerUtils.GetEventTypeForListenerType<TListener>();
            if (!_gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData)) 
            {
                return;
            }
            
            int removalIndex = listenersWithData.IndexOf(listenerInstance);
            if (removalIndex < 0)
            {
                return;
            }
                
            // Remove the listener
            listenersWithData.RemoveAt(removalIndex);
            if (listenersWithData.Count == 0)
            {
                _gameEventWithDataListeners.Remove(gameEventWithDataType);
                _currentEventRaiseIterations.Remove(gameEventWithDataType);
            }
            
            if (--_listenerInstances[listenerInstance] == 0)
            {
                _listenerInstances.Remove(listenerInstance);
                _dataListenerToEventRaises.Remove(listenerInstance);
            }
            
            // Removal might affect a current event raise
            UpdateIterationsWithRemovedListener(gameEventWithDataType, removalIndex);
        }

        public void RaiseEventWithData<T>(T gameEventWithData) where T : GameEventWithData
        {
            Type gameEventWithDataType = typeof(T);  
            if (!_gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out List<IGameEventWithDataListener> listenersWithData))
            {
                Debug.LogWarning($"Raised event `{gameEventWithDataType}` with no listeners");
                return;
            }

            if (!_currentEventRaiseIterations.TryGetValue(gameEventWithDataType, out Dictionary<long, int> iterations))
            {
                iterations = new Dictionary<long, int>();
                _currentEventRaiseIterations.Add(gameEventWithDataType, iterations);
            }

            long iterationKey = _numEventRaises++;
            iterations.Add(iterationKey, 0);

            for (iterations[iterationKey] = 0; _currentEventRaiseIterations.ContainsKey(gameEventWithDataType) && iterations[iterationKey] < listenersWithData.Count; iterations[iterationKey]++)
            {
                _dataListenerToEventRaises[listenersWithData[iterations[iterationKey]]][gameEventWithDataType].Invoke(gameEventWithData.RawData);
            }
            iterations.Remove(iterationKey);
        }

        public void RaiseEventWithoutData<T>(T gameEventWithoutData) where T : GameEventWithoutData
        {
            Type gameEventWithoutDataType = typeof(T);  
            if (!_gameEventWithoutDataListeners.TryGetValue(gameEventWithoutDataType, out List<IGameEventWithoutDataListener> listenersWithoutData))
            {
                Debug.LogWarning($"Raised event `{gameEventWithoutDataType}` with no listeners");
                return;
            }

            if (!_currentEventRaiseIterations.TryGetValue(gameEventWithoutDataType, out Dictionary<long, int> iterations))
            {
                iterations = new Dictionary<long, int>();
                _currentEventRaiseIterations.Add(gameEventWithoutDataType, iterations);
            }

            long iterationKey = _numEventRaises++;
            iterations.Add(iterationKey, 0);

            for (iterations[iterationKey] = 0; _currentEventRaiseIterations.ContainsKey(gameEventWithoutDataType) && iterations[iterationKey] < listenersWithoutData.Count; iterations[iterationKey]++)
            {
                _noDataListenerToEventRaises[listenersWithoutData[iterations[iterationKey]]][gameEventWithoutDataType].Invoke();
            }
            iterations.Remove(iterationKey);
        }
        
        public bool TryGetListenersForNonDataEvent<TEventWithoutData>(out List<IGameEventWithoutDataListener> listeners) where TEventWithoutData : GameEventWithoutData
        {
            Type gameEventType = typeof(TEventWithoutData);
            if (_currentEventRaiseIterations.ContainsKey(gameEventType))
            {
                Debug.LogWarning("Possibly modification of listener list during an active event");
            }
            
            return _gameEventWithoutDataListeners.TryGetValue(gameEventType, out listeners);
        }
        
        public bool TryGetListenersForDataEvent<TEventWithData>(out List<IGameEventWithDataListener> dataListeners) where TEventWithData : GameEventWithData
        {
            Type gameEventWithDataType = typeof(TEventWithData);
            if (_currentEventRaiseIterations.ContainsKey(gameEventWithDataType))
            {
                Debug.LogWarning("Possibly modification of listener list during an active event");
            }
            
            return _gameEventWithDataListeners.TryGetValue(gameEventWithDataType, out dataListeners);
        }
        
        private void UpdateIterationsWithRemovedListener(Type gameEventType, int indexRemovedListener)
        {
            if (!_currentEventRaiseIterations.TryGetValue(gameEventType, out Dictionary<long, int> iterations))
            {
                return;
            }
            
            List<(long iterationKey, int newIteration)> newIterations = null;
            foreach ((long iterationKey, int iteration) in iterations)
            {
                if (iteration >= indexRemovedListener)
                {
                    (newIterations ??= new List<(long iterationKey, int newIteration)>()).Add((iterationKey, iteration - 1));
                }
            }

            if (newIterations != null)
            {
                foreach ((long iterationKey, int newIteration) in newIterations)
                {
                    iterations[iterationKey] = newIteration;
                }   
            }
        }
        
        private static Type GetImplementedInterfaceType(Type type, string interfaceName)
        {
            foreach (Type interfaceType in type.GetInterfaces().Where(i => i.IsGenericType))
            {
                if (interfaceType.GetGenericTypeDefinition().Name.StartsWith(interfaceName, StringComparison.OrdinalIgnoreCase))
                {
                    return interfaceType;
                }
            }
            return null;
        }
    }
}