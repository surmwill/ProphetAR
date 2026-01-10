using System.Collections;
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
            yield return new WaitForAllCoroutines(CoroutineOne(), CoroutineTwo());
            Debug.Log("All coroutines done");
        }
        
        private IEnumerator CoroutineOne()
        {
            yield return new WaitForSeconds(2f);
            Debug.Log("Coroutine one done");
        }

        private IEnumerator CoroutineTwo()
        {
            yield return new WaitForSeconds(5f);
            Debug.Log("Coroutine two done");
        }
    }
}