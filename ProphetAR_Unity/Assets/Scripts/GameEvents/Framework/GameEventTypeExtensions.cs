using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProphetAR
{
    public static class GameEventTypeExtensions
    {
        private static readonly Dictionary<GameEventType, Type> CachedListenerInterfaces = new();
        
        public static Type GetInterfaceOfListeners(this GameEventType gameEventType)
        {
            if (!CachedListenerInterfaces.TryGetValue(gameEventType, out Type listenerInterface))
            {
                FieldInfo fieldInfo = typeof(GameEventType).GetField(gameEventType.ToString());
                HasGameEventListenersOfTypeAttribute attribute = fieldInfo.GetCustomAttribute<HasGameEventListenersOfTypeAttribute>(false);
                
                if (attribute == null)
                {
                    Debug.LogWarning($"Missing event handler for `{gameEventType}`. Please specify as an attribute in `{nameof(GameEventType)}`");
                    return null;
                }
                
                listenerInterface = attribute.TypeGameEventListeners;
                CachedListenerInterfaces.Add(gameEventType, listenerInterface);
            }

            return listenerInterface;
        }
    }
}