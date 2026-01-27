using UnityEngine;

namespace ProphetAR
{
    public class ARRaycastDrawerLine : ARRaycastDrawer
    {
        [SerializeField]
        private LineRenderer _raycastLine = null;

        private Camera ARCamera => ARManager.Instance.Camera;
        
        private static readonly Vector3[] LinePositions = new Vector3[2]; 
        
        public override void DrawRaycast(RaycastHit raycastHit)
        {
            LinePositions[0] = ARCamera.transform.position + ARCamera.transform.up * -0.1f;
            LinePositions[1] = raycastHit.point;

            Transform groundPlaneContent = ARManager.Instance.GroundPlaneManager.Content;
            _raycastLine.transform.up = groundPlaneContent != null ? -groundPlaneContent.up : -Vector3.up;   
            
            _raycastLine.SetPositions(LinePositions);
        }

        private void OnValidate()
        {
            if (_raycastLine == null)
            {
                _raycastLine = GetComponent<LineRenderer>();
            }
        }
    }
}