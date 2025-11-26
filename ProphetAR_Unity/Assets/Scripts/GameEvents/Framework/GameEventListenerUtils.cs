using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProphetAR
{
    public static class GameEventListenerUtils
    {
        private static readonly Dictionary<Type, Type> ListenerTypeToEventType = new();
        
        public static Type GetEventTypeForListenerType<T>() where T : IGameEventListener
        {
            Type listenerType = typeof(T);
            
            if (!ListenerTypeToEventType.TryGetValue(listenerType, out Type eventType))
            {
                ListensToGameEventTypeAttribute attribute = listenerType.GetCustomAttribute<ListensToGameEventTypeAttribute>(false);
                
                if (attribute == null)
                {
                    Debug.LogWarning($"A game event listener of type `{nameof(T)}` is missing a corresponding game event. Please specify it as an attribute in its class.");
                    return null;
                }

                eventType = attribute.TypeGameEvent;
                ListenerTypeToEventType.Add(listenerType, eventType);
            }

            return eventType;
        }
    }
}