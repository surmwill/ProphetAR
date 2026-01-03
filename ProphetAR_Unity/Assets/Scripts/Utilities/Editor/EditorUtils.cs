#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProphetAR.Editor
{
    /// <summary>
    /// Helpful editor functions.
    /// </summary>
    public static class EditorUtils
    {
        /// <summary>
        /// Destroy is only allowed during runtime and the alternative, DestroyImmediate, is not allowed in OnValidate.
        /// If we wish to destroy something during OnValidate we'll need to move the actual destruction outside of the call.
        /// </summary>
        /// <param name="obj"> The object to destroy </param>
        public static void DestroyOnValidate(Object obj)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                EditorApplication.delayCall += () => Object.DestroyImmediate(obj);   
            }
        }
    }
}
#endif