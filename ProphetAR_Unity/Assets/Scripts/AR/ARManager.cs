using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ProphetAR
{
    public class ARManager : Singleton<ARManager>
    {
        [SerializeField]
        private ARRaycastManager _raycastManager = null;

        [SerializeField]
        private ARAnchorManager _anchorManager = null;

        [SerializeField]
        private ARCameraManager _cameraManager = null;

        [SerializeField]
        private GroundPlaneManager _groundPlaneManager = null;

        [SerializeField]
        private ARSession _arSession = null;

        public ARRaycastManager RaycastManager => _raycastManager;

        public ARAnchorManager AnchorManager => _anchorManager;

        public ARCameraManager CameraManager => _cameraManager;

        public GroundPlaneManager GroundPlaneManager => _groundPlaneManager;
        
        public ARSession ARSession => _arSession;

        public Camera ARCamera
        {
            get
            {
                if (_arCamera == null)
                {
                    _arCamera = CameraManager.GetComponent<Camera>();
                }

                return _arCamera;
            }
        }

        private Camera _arCamera;
    }
}
