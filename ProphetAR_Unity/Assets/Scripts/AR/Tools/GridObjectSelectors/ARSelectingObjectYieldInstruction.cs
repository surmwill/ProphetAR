using UnityEngine;

namespace ProphetAR
{
    public class ARSelectingObjectYieldInstruction : CustomYieldInstruction
    {
        public override bool keepWaiting => !Selected && !Cancelled;
        
        public bool Selected { get; set; }
        
        public bool Cancelled { get; set; }
    }
}