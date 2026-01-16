using System;
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
            Debug.Log("Coroutines started");
            yield return null;
        }
    }
}