using System;
using System.Reflection;

namespace ProphetAR
{
    public class CachedGameEventWithDataListener : IEquatable<CachedGameEventWithDataListener>
    {
        public IGameEventWithDataListener Listener { get; }
        
        private readonly MethodInfo _cachedRaiseEventMethod;

        public CachedGameEventWithDataListener(IGameEventWithDataListener listener, Type listenerInterfaceType)
        {
            Listener = listener;
            _cachedRaiseEventMethod = listenerInterfaceType.GetMethod(IGameEventListener.OnEventMethodName);
        }

        public void RaiseEvent(object rawData)
        {
            _cachedRaiseEventMethod.Invoke(Listener, new [] { rawData });
        }
        
        public bool Equals(CachedGameEventWithDataListener other)
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
            
            return Equals((CachedGameEventWithDataListener) obj);
        }

        public override int GetHashCode()
        {
            return Listener != null ? Listener.GetHashCode() : 0;
        }

        public static bool operator ==(CachedGameEventWithDataListener left, CachedGameEventWithDataListener right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CachedGameEventWithDataListener left, CachedGameEventWithDataListener right)
        {
            return !Equals(left, right);
        }
    }
}