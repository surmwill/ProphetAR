using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public class WaitForAllCoroutines : CustomYieldInstruction
    {
        private readonly List<IEnumerator> _coroutines;
        
        public WaitForAllCoroutines(params IEnumerator[] coroutines)
        {
            _coroutines = coroutines.ToList();
        }
        
        public WaitForAllCoroutines(IEnumerable<IEnumerator> coroutines)
        {
            _coroutines = coroutines.ToList();
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