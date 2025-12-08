using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public interface IGameTurnActionRequest
    {
        public int? Priority { get; }
        
        public bool IsNecessary { get; }
        
        public Transform FocusTransform { get; }

        public void OnFocus(Transform focusTransform);

        public void AIExecute();

        public Dictionary<string, object> SerializeForServer();
    }
}