using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProphetAR
{
    public static class GameEventProcessor
    {
        private static readonly LinkedList<GameEvent> GameEventQueue = new();
        
        private static readonly Dictionary<GameEventType, LinkedList<CachedGameEventWithoutDataListener>> GameEventWithoutDataListeners = new();
        private static readonly Dictionary<GameEventType, LinkedList<CachedGameEventWithDataListener>> GameEventWithDataListeners = new();

        // Remember to shift these if we delete
        private static LinkedListNode<CachedGameEventWithoutDataListener> _currentlyHandlingListenerWithoutDataNode;
        private static LinkedListNode<CachedGameEventWithDataListener> _currentlyHandlingListenerWithDataNode;

        public static void RaiseEvent(GameEvent gameEvent)
        {
            // Data
            if (gameEvent is GameEventWithData gameEventWithData)
            {
                if (!GameEventWithDataListeners.TryGetValue(gameEvent.GameEventType, out LinkedList<CachedGameEventWithDataListener> listenersWithDataQueue) || listenersWithDataQueue.Count == 0)
                {
                    Debug.LogWarning($"Raised event `{gameEvent.GameEventType}` with no listeners");
                    return;
                }

                _currentlyHandlingListenerWithDataNode = listenersWithDataQueue.First;
                while (_currentlyHandlingListenerWithDataNode != null)
                {
                    CachedGameEventWithDataListener listener = _currentlyHandlingListenerWithDataNode.Value;
                    listener.RaiseEvent(gameEventWithData.RawData);
                    _currentlyHandlingListenerWithDataNode = _currentlyHandlingListenerWithDataNode.Next;
                }

                return;
            }
            
            // No data
            if (!(gameEvent is GameEventWithoutData gameEventWithoutData))
            {
                Debug.LogWarning($"Error handling event `{gameEvent.GameEventType}`. All game events must derive from GameEventWithoutData or GameEventWithTypedData. " +
                                 $"Check the associated listener in `${nameof(GameEventType)}`");
                return;
            }
            
            
            if (!GameEventWithoutDataListeners.TryGetValue(gameEvent.GameEventType, out LinkedList<CachedGameEventWithoutDataListener> listenersQueue))
            {
                Debug.LogWarning($"Raised event `{gameEvent.GameEventType}` with no listeners");
                return;
            }

            _currentlyHandlingListenerWithoutDataNode = listenersQueue.First;
            while (_currentlyHandlingListenerWithoutDataNode != null)
            {
                CachedGameEventWithoutDataListener listener = _currentlyHandlingListenerWithoutDataNode.Value;
                listener.RaiseEvent();
                _currentlyHandlingListenerWithoutDataNode = _currentlyHandlingListenerWithoutDataNode.Next;
            }
        }
    }
}