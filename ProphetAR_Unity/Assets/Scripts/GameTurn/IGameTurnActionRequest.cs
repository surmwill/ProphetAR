using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public interface IGameTurnActionRequest
    {
        public const int DefaultPriority = int.MaxValue;
        
        public int? Priority { get; }
        
        public Transform FocusTransform { get; }

        public void OnFocus(Transform focusTransform);

        public void ExecuteAutomatically();

        public Dictionary<string, object> SerializeForServer();
    }
}