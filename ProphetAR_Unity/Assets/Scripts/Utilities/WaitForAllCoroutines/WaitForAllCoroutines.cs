using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
        public class WaitForAllCoroutines : CustomYieldInstruction
        {
            public static WaitForAllCoroutinesCoroutineRunner CoroutineRunner { get; private set; }
            
            public override bool keepWaiting => _numCoroutines > 0;
            
            private int _numCoroutines;
            
            private readonly List<IEnumerator> _coroutines;
            
            public WaitForAllCoroutines(params IEnumerator[] coroutines)
            {
                Start(coroutines);
            }
            
            public WaitForAllCoroutines(MonoBehaviour coroutineRunner, params IEnumerator[] coroutines)
            {
                Start(coroutines, coroutineRunner);
            }
            
            public WaitForAllCoroutines(IEnumerable<IEnumerator> coroutines)
            {
                Start(coroutines);
            }
            
            public WaitForAllCoroutines(IEnumerable<IEnumerator> coroutines, MonoBehaviour coroutineRunner = null)
            {
                Start(coroutines, coroutineRunner);
            }

            private void Start(IEnumerable<IEnumerator> coroutines, MonoBehaviour coroutineRunner = null)
            {
                if (coroutineRunner == null)
                {
                    if (CoroutineRunner == null)
                    {
                        CoroutineRunner = new GameObject("CoroutineRunner [WaitForAllCoroutines]",
                            typeof(WaitForAllCoroutinesCoroutineRunner)).GetComponent<WaitForAllCoroutinesCoroutineRunner>();
                        
                        Object.DontDestroyOnLoad(CoroutineRunner);
                    }

                    coroutineRunner = CoroutineRunner;
                }
                
                foreach (IEnumerator coroutine in coroutines)
                {
                    _numCoroutines++;
                    coroutineRunner.StartCoroutine(WrapCoroutine(coroutine));
                }
            }

            private IEnumerator WrapCoroutine(IEnumerator coroutine)
            {
                yield return coroutine;
                _numCoroutines--;
            }
        }
}