using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ProphetAR
{
    public class GroundPlaneManager : MonoBehaviour
    {
        [SerializeField]
        private GroundPlane _groundPlanePrefab = null;
        
        [SerializeField]
        private GameObject _defaultGroundPlanePlacementIndicatorPrefab = null;
        
        [Header("Debug")]
        [SerializeField]
        private GameObject _debugHitTestIndicatorPrefab = null;

        public Transform Content => _groundPlane == null ? null : _groundPlane.Content;
        
        private GroundPlane _groundPlane;
        private Transform _prevGroundPlaneContent;
        
        private GameObject _initialContentPrefab;

        private Coroutine _scanRoomCoroutine;
        private Coroutine _updateGroundPlanePlacementCoroutine;

        private ARRaycastHit _lastPlacementRaycast;
        private GameObject _placementIndicator;
        private readonly List<GameObject> _debugScanRoomHitIndicators = new();

        private ARAnchor _groundPlaneAnchor;

        public void ScanRoomForPlanes(Action<float> onProgress, Action onComplete, bool debugShowHits = false)
        {
            CancelScanningRoom();
            _scanRoomCoroutine = StartCoroutine(ScanRoom(onProgress, onComplete, debugShowHits));
        }
        
        private IEnumerator ScanRoom(Action<float> onProgress, Action onComplete, bool debugShowHits)
        {
            const float Interval = 0.1f;
            const float MinDistance = 0.1f;
            const int NumHitsRequired = 5; // 20

            List<Pose> prevHitTests = new List<Pose>();
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            onProgress?.Invoke(0f);
            while (prevHitTests.Count < NumHitsRequired)
            {
                if (ARManager.Instance.RaycastManager.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose currentHit = hits[0].pose;
                    if (!prevHitTests.Any(prevHit => (prevHit.position - currentHit.position).sqrMagnitude < MinDistance))
                    {
                        prevHitTests.Add(currentHit);
                        onProgress?.Invoke((float) prevHitTests.Count / NumHitsRequired);

                        if (debugShowHits)
                        {
                            _debugScanRoomHitIndicators.Add(Instantiate(_debugHitTestIndicatorPrefab, currentHit.position, currentHit.rotation));
                        }
                    }
                }

                yield return new WaitForSeconds(Interval);
            }
            
            CancelScanningRoom();
            onComplete?.Invoke();
        }

        public void CancelScanningRoom()
        {
            if (_scanRoomCoroutine != null)
            {
                StopCoroutine(_scanRoomCoroutine);
                _scanRoomCoroutine = null;
            }
            
            foreach (GameObject hitIndicator in _debugScanRoomHitIndicators)
            {
                Destroy(hitIndicator);
            }
            _debugScanRoomHitIndicators.Clear();
        }
        
        // Begins raycasting on the ground and showing where the ground plane would be placed. The user can then move around and decide the exact location
        public void StartGroundPlanePlacement(GameObject initialContentPrefab = null, GameObject customPlacementIndicatorPrefab = null, bool usePreviousContent = false)
        {
            if (_groundPlane != null)
            {
                throw new InvalidOperationException("Ground plane has already been placed. Please destroy the existing one first");
            }
            
            CancelGroundPlanePlacement();

            if (usePreviousContent && _prevGroundPlaneContent == null)
            {
                Debug.LogWarning("No previous ground plane content to use");
            }
            else if (!usePreviousContent && _prevGroundPlaneContent != null)
            {
                Destroy(_prevGroundPlaneContent.gameObject);
                _prevGroundPlaneContent = null;
            }
            
            GameObject placementIndicator = customPlacementIndicatorPrefab == null ? _defaultGroundPlanePlacementIndicatorPrefab : customPlacementIndicatorPrefab;
            _initialContentPrefab = initialContentPrefab;
            
            _updateGroundPlanePlacementCoroutine = StartCoroutine(UpdateGroundPlanePlacement(placementIndicator));
        }
        
        public void CancelGroundPlanePlacement()
        {
            if (_updateGroundPlanePlacementCoroutine != null)
            {
                StopCoroutine(_updateGroundPlanePlacementCoroutine);
                _updateGroundPlanePlacementCoroutine = null;
            }
           
            if (_placementIndicator != null)
            {
                Destroy(_placementIndicator);
                _placementIndicator = null;
            }
            
            _initialContentPrefab = null;
        }
        
        // Places the ground plane in the currently indicated position
        public bool TryPlaceGroundPlane()
        {
            if (_placementIndicator == null)
            {
                Debug.LogWarning("Ground plane placement requires at least one successful hit test");
                return false;
            }

            ARPlane hitPlane = ARManager.Instance.PlaneManager.GetPlane(_lastPlacementRaycast.trackableId);
            Pose hitPose = _lastPlacementRaycast.pose;
            _groundPlaneAnchor = ARManager.Instance.AnchorManager.AttachAnchor(hitPlane, hitPose);
            
            _groundPlane = Instantiate(_groundPlanePrefab, _groundPlaneAnchor.transform);
            _groundPlane.transform.SetPositionAndRotation(_placementIndicator.transform.position, _placementIndicator.transform.rotation);
            
            if (_prevGroundPlaneContent != null)
            {
                _groundPlane.Content = _prevGroundPlaneContent;
                _prevGroundPlaneContent = null;
            }

            if (_initialContentPrefab != null)
            {
                GameObject initialContent = GroundedInstantiate(_initialContentPrefab);
                
                IARContentPlacementPositioner contentPositioner = initialContent.GetComponent<IARContentPlacementPositioner>();
                if (contentPositioner != null)
                {
                    contentPositioner.PositionContent();
                }
                
                _initialContentPrefab = null;
            }
            
            CancelGroundPlanePlacement();
            return true;
        }
        
        private IEnumerator UpdateGroundPlanePlacement(GameObject placementIndicatorPrefab)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            
            for (;;)
            {
                if (ARManager.Instance.RaycastManager.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), hits, TrackableType.PlaneWithinPolygon))
                {
                    ARRaycastHit hit = hits[0];
                    Pose hitPose = hit.pose;
                    Vector3 faceAwayFromUser = Vector3.ProjectOnPlane(hitPose.position - ARManager.Instance.Camera.transform.position, hitPose.up);
                    
                    if (_placementIndicator == null)
                    {
                        _placementIndicator = Instantiate(placementIndicatorPrefab);
                    }
                    
                    _placementIndicator.transform.SetPositionAndRotation(hitPose.position, Quaternion.LookRotation(faceAwayFromUser, hitPose.up));
                    _lastPlacementRaycast = hit;
                }
                
                yield return null;
            }
        }

        public void DestroyGroundPlane(bool preservePreviousContent = false)
        {
            if (preservePreviousContent && _groundPlane != null)
            {
                _prevGroundPlaneContent = _groundPlane.Content;
                _prevGroundPlaneContent.SetParent(null);
            }
            else if (!preservePreviousContent && _prevGroundPlaneContent != null)
            {
                Destroy(_prevGroundPlaneContent.gameObject);
                _prevGroundPlaneContent = null;
            }
            
            if (_groundPlane != null)
            {
                Destroy(_groundPlane.gameObject);
                _groundPlane = null;   
            }

            if (_groundPlaneAnchor != null)
            {
                ARManager.Instance.AnchorManager.TryRemoveAnchor(_groundPlaneAnchor);
                _groundPlaneAnchor = null;
            }
        }

        public bool RaycastGroundPlane(Vector2 normalizedScreenPosition, out RaycastHit hit)
        {
            if (_groundPlane == null)
            {
                throw new InvalidOperationException("Ground plane does not exist");
            }

            Ray ray = ARManager.Instance.Camera.ScreenPointToRay(new Vector2(Screen.width * normalizedScreenPosition.x, Screen.height * normalizedScreenPosition.y));
            return Physics.Raycast(ray, out hit, LayerMask.GetMask(GroundPlane.Layer));
        }
        
        public GameObject GroundedInstantiate(GameObject prefab, bool instantiateInWorldSpace = false)
        {
            if (_groundPlane == null)
            {
                throw new InvalidOperationException("Ground plane does not exist");
            }
            
            return Instantiate(prefab, _groundPlane.Content, instantiateInWorldSpace);
        }

        public void ParentToGround(Transform t, bool worldPositionStays = true)
        {
            if (_groundPlane == null)
            {
                throw new InvalidOperationException("Ground plane does not exist");
            }
            
            t.SetParent(_groundPlane.Content, worldPositionStays);
        }
        
        private void OnDestroy()
        {
            CancelScanningRoom();
            CancelGroundPlanePlacement();
        }
    }
}
