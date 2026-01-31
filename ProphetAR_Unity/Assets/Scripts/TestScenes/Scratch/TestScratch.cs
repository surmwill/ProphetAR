using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// A scratch file for when you need to test or verify something simple that does not need to be preserved
    /// </summary>
    public class TestScratch : MonoBehaviour
    {
        [SerializeField]
        private ARContentPlacementCenterOnGridCell _placement = null;

        private IEnumerator Start()
        {
            Debug.Log("Started " + Time.frameCount);
            yield return new WaitForAllCoroutines(new []{ TestA(), TestB() });
            Debug.Log("Ended " + Time.frameCount);
        }

        private IEnumerator TestA()
        {
            Debug.Log("CoroutineA " + Time.frameCount);
            yield return null;
            Debug.Log("CoroutineA " + Time.frameCount);
            yield return null;
            Debug.Log("CoroutineA " + Time.frameCount);
        }
        
        private IEnumerator TestB()
        {
            Debug.Log("CoroutineB " + Time.frameCount);
            yield return null;
            Debug.Log("CoroutineB " + Time.frameCount);
            yield return null;
            Debug.Log("CoroutineB " + Time.frameCount);
        }
    }
}