using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
        public class WaitForAllCoroutines : CustomYieldInstruction
        {
            public override bool keepWaiting
            {
                get
                {
                    List<IEnumerator> toRemove = null;
                    foreach (IEnumerator coroutine in _coroutines)
                    {
                        if (!coroutine.MoveNext())
                        {
                            (toRemove ??= new()).Add(coroutine);
                        }
                    }

                    if (toRemove != null)
                    {
                        foreach (IEnumerator coroutine in toRemove)
                        {
                            _coroutines.Remove(coroutine);
                        }
                    }
                    
                    return _coroutines.Count > 0;
                }
            }
            
            private readonly List<IEnumerator> _coroutines;
            
            public WaitForAllCoroutines(IEnumerable<IEnumerator> coroutines)
            {
                _coroutines = coroutines.ToList();
            }
            
            public WaitForAllCoroutines(params IEnumerator[] coroutines) : this(coroutines.AsEnumerable()) { }
        }
}