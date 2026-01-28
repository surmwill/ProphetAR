using UnityEngine;

namespace ProphetAR
{
    public class ARRaycastDrawerObject : ARRaycastDrawer
    {
        public override void DrawRaycast(RaycastHit raycastHit)
        {
            Vector3 up = ARManager.Instance.GroundPlaneManager.Content != null ? ARManager.Instance.GroundPlaneManager.Content.up : Vector3.up;
            transform.position = raycastHit.point + up * 0.1f;
            transform.forward = Vector3.ProjectOnPlane(raycastHit.point - ARManager.Instance.Camera.transform.position, up);
        }
    }
}