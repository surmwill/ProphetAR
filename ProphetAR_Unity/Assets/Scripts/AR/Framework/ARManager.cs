using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ProphetAR
{
    public class ARManager : Singleton<ARManager>, ILevelLifecycleListener
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
            
            Level.RegisterLevelLifecycleListener(this);
            
            _arGridCellSelector.Initialize(Camera);   
        }

        protected override void OnDestroy()
        {
            Level.UnregisterLevelLifecycleListener(this);
            
            base.OnDestroy();
        }

        public void OnLevelLifecycleChanged(LevelLifecycleState lifecycleState, Level prevLevel, Level currLevel)
        {
            if (lifecycleState == LevelLifecycleState.Destroyed)
            {
                _arGridCellSelector.Cancel();
            }
        }
    }
}
