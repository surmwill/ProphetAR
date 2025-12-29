using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    public class WaitForAllCoroutines : CustomYieldInstruction
    {
        private readonly List<IEnumerator> _coroutines;
        
        public WaitForAllCoroutines(List<IEnumerator> coroutines)
        {
            _coroutines = coroutines;
        }

        public override bool keepWaiting
        {
            get
            {
                for (int i = _coroutines.Count - 1; i >= 0; i--)
                {
                    if (!_coroutines[i].MoveNext())
                    {
                        _coroutines.RemoveAt(i);
                    }
                }

                return _coroutines.Count > 0;
            }
        }
    }
}