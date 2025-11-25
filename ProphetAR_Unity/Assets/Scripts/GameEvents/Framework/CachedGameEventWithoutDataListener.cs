using System;
using System.Reflection;

namespace ProphetAR
{
    public class CachedGameEventWithoutDataListener : IEquatable<CachedGameEventWithoutDataListener>
    {
        public IGameEventWithoutDataListener Listener { get; }
        
        private readonly MethodInfo _cachedRaiseEventMethod;
        
        public CachedGameEventWithoutDataListener(IGameEventWithoutDataListener listener, Type listenerInterfaceType)
        {
            Listener = listener;
            _cachedRaiseEventMethod = listenerInterfaceType.GetMethod(IGameEventListener.OnEventMethodName);
        }
        
        public void RaiseEvent()
        {
            _cachedRaiseEventMethod.Invoke(Listener, Array.Empty<object>());
        }
        
        public bool Equals(CachedGameEventWithoutDataListener other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }
            
            return Equals(Listener, other.Listener);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }
            
            return Equals((CachedGameEventWithoutDataListener) obj);
        }

        public override int GetHashCode()
        {
            return Listener != null ? Listener.GetHashCode() : 0;
        }

        public static bool operator ==(CachedGameEventWithoutDataListener left, CachedGameEventWithoutDataListener right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CachedGameEventWithoutDataListener left, CachedGameEventWithoutDataListener right)
        {
            return !Equals(left, right);
        }
    }
}