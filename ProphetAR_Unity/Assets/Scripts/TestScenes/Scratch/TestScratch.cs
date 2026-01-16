using System;
using System.Collections;
using ProphetAR.Coroutines;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// A scratch file for when you need to test or verify something simple that does not need to be preserved
    /// </summary>
    public class TestScratch : MonoBehaviour
    {
        private IEnumerator Start()
        {
            Debug.Log("Coroutines started");
            yield return new WaitForCallback(Foo);
            Debug.Log("All coroutines done");
        }

        private void Foo(Action onComplete)
        {
            StartCoroutine(FooInner(onComplete));
        }

        private IEnumerator FooInner(Action onComplete)
        {
            yield return new WaitForSeconds(5f);
            onComplete?.Invoke();
        }
    }
}