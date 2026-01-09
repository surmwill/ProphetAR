using UnityEngine;

#if UNITY_EDITOR
using ProphetAR.Editor;
#endif

namespace ProphetAR
{
    public static class DestroyUtils
    {
        /// <summary>
        /// Destroys an object. Safe to call in or out of the editor, while the game is playing or not.
        /// If you are sure that the object will be destroyed while the game is playing, use the normal Destroy. Otherwise, use this.
        /// </summary>
        public static void DestroyAnywhere(Object obj)
        {
            #if UNITY_EDITOR
            EditorUtils.DestroyOnValidate(obj);
            #else
            Object.Destroy(obj);
            #endif
        }

        public static void DestroyAnywhereChildren(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            
            foreach (Transform child in gameObject.transform)
            {
                DestroyAnywhere(child.gameObject);
            }
        }
    }
}