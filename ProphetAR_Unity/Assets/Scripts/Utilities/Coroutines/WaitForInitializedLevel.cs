using UnityEngine;

namespace ProphetAR.Coroutines
{
    public class WaitForInitializedLevel : CustomYieldInstruction
    {
        public override bool keepWaiting => Level.Current == null || !Level.Current.IsInitialized;
    }
}