using System;

namespace ProphetAR
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HasGameEventListenersOfTypeAttribute : Attribute
    { 
        public Type TypeGameEventListeners { get; }

        private static readonly Type TypeBaseGameEventListener = typeof(IGameEventListener);  
        
        public HasGameEventListenersOfTypeAttribute(Type typeGameEventListeners)
        {
            if (!TypeBaseGameEventListener.IsAssignableFrom(typeGameEventListeners))
            {
                throw new ArgumentException($"Expected an `{nameof(IGameEventListener)}` type as the specified event responder.");
            }
            
            TypeGameEventListeners = typeGameEventListeners;
        }
    }
}