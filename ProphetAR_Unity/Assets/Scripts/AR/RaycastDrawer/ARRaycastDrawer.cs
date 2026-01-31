using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ProphetAR
{
    public abstract class ARRaycastDrawer : MonoBehaviour
    {
        public virtual void DrawRaycast(RaycastHit raycastHit, Ray fromRay)
        {
            // Empty
        }

        public virtual void DrawRaycast(ARRaycastHit raycastHit, Ray fromRay)
        {
            // Empty
        }
    }
}