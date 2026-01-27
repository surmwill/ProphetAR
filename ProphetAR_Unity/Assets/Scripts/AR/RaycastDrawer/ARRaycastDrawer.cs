using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ProphetAR
{
    public abstract class ARRaycastDrawer : MonoBehaviour
    {
        public virtual void DrawRaycast(RaycastHit raycastHit)
        {
            // Empty
        }

        public virtual void DrawRaycast(ARRaycastHit raycastHit)
        {
            // Empty
        }
    }
}