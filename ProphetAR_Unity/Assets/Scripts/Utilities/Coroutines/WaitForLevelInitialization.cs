using UnityEngine;

namespace ProphetAR.Coroutines
{
    public class WaitForLevelInitialization : CustomYieldInstruction
    {
        private readonly Level _level;

        public override bool keepWaiting => !_level.IsInitialized;

        public WaitForLevelInitialization(Level level)
        {
            _level = level;
        }
    }
}