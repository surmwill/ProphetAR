using System;
using UnityEngine;

namespace ProphetAR
{
    public class WaitForARObjectSelection<T> : CustomYieldInstruction where T : class
    {
        public override bool keepWaiting => IsWaiting;

        public bool IsResolved => ResolvedSelected || ResolvedCancelled;
        
        public bool IsWaiting => !ResolvedSelected && !ResolvedCancelled;
        
        public T SelectedObject { get; private set; }
        
        public bool ResolvedSelected { get; private set; }
        
        public bool ResolvedCancelled { get; private set; }
        
        public ARObjectSelector<T> Selector { get; private set; }
        
        public WaitForARObjectSelection(ARObjectSelector<T> objectSelector)
        {
            Selector = objectSelector;
        }

        public void SetResolvedSelected(T selectedObject)
        {
            if (ResolvedCancelled || ResolvedSelected)
            {
                throw new InvalidOperationException($"AR selection has already been resolved ({(ResolvedSelected ? "successfully" : "cancelled")})");
            }
            
            SelectedObject = selectedObject;
            ResolvedSelected = true;
        }

        public void SetResolvedCancelled()
        {
            if (ResolvedCancelled || ResolvedSelected)
            {
                throw new InvalidOperationException($"AR selection has already been resolved ({(ResolvedSelected ? "successfully" : "cancelled")})");
            }

            ResolvedCancelled = true;
        }
    }
}