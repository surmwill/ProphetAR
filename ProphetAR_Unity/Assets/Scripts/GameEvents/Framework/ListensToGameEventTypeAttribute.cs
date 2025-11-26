using System;

namespace ProphetAR
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ListensToGameEventTypeAttribute : Attribute
    { 
        public Type TypeGameEvent { get; }
        
        public ListensToGameEventTypeAttribute(Type typeGameEvent)
        {
            if (!typeof(GameEvent).IsAssignableFrom(typeGameEvent))
            {
                throw new ArgumentException($"`{typeGameEvent.Name}` must be a `{nameof(GameEvent)}` type.");
            }
            
            TypeGameEvent = typeGameEvent;
        }
    }
}