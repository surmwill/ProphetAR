using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

        public ARRaycastManager RaycastManager => _raycastManager;

        public ARAnchorManager AnchorManager => _anchorManager;

        public ARCameraManager CameraManager => _cameraManager;

        public GroundPlaneManager GroundPlaneManager => _groundPlaneManager;

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
