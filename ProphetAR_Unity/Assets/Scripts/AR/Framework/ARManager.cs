using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ProphetAR
{
    public class ARManager : Singleton<ARManager>
    {
        [Header("Framework")]
        [SerializeField]
        private ARRaycastManager _raycastManager = null;

        [SerializeField]
        private ARAnchorManager _anchorManager = null;

        [SerializeField]
        private ARCameraManager _cameraManager = null;
        
        [SerializeField]
        private ARSession _arSession = null;

        [SerializeField]
        private ARPlaneManager _planeManager = null;
        
        [SerializeField]
        private GroundPlaneManager _groundPlaneManager = null;
        
        [Header("Tools")]
        [SerializeField]
        private ARGridCellSelector _arGridCellSelector;

        public ARRaycastManager RaycastManager => _raycastManager;

        public ARAnchorManager AnchorManager => _anchorManager;

        public ARCameraManager CameraManager => _cameraManager;
        
        public ARSession ARSession => _arSession;

        public ARPlaneManager PlaneManager => _planeManager;
        
        public GroundPlaneManager GroundPlaneManager => _groundPlaneManager;

        public ARGridCellSelector ARGridCellSelector => _arGridCellSelector;

        public Camera Camera
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

        protected override void Awake()
        {
            base.Awake();
            
            _arGridCellSelector.Initialize(Camera);   
        }
    }
}
