#if UNITY_EDITOR
using ProphetAR.Editor;
#endif

using UnityEngine;

namespace ProphetAR
{
    public static class DestroyUtils
    {
        /// <summary>
        /// Destroys an object. Safe to call in or out of the editor, while the game is playing or not
        /// </summary>
        public static void SafeDestroy(Object obj)
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                EditorUtils.DestroyInEditMode(obj);
            }
            else
            {
                Object.Destroy(obj);
            }
            #else
            Object.Destroy(obj);
            #endif
        }
    }
}