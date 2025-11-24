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
        
        public Transform Content => _groundPlane.Content;

        private GroundPlane _groundPlane;
        private Transform _prevContent;

        private Coroutine _scanRoomCoroutine;
        private Coroutine _updateGroundPlanePlacementCoroutine;
        
        private GameObject _placementIndicator;
        private readonly List<GameObject> _debugScanRoomHitIndicators = new();

        public void ScanRoomForPlanes(Action<float> onProgress, Action onComplete)
        {
            CancelScanningRoom();
            _scanRoomCoroutine = StartCoroutine(ScanRoom(onProgress, onComplete));
        }
        
        private IEnumerator ScanRoom(Action<float> onProgress, Action onComplete)
        {
            const float Interval = 0.1f;
            const float MinDistance = 0.1f;
            const int NumHitsRequired = 5; // 20

            List<Pose> prevHitTests = new List<Pose>();
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            while (prevHitTests.Count < NumHitsRequired)
            {
                if (ARManager.Instance.RaycastManager.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose currentHit = hits[0].pose;
                    if (!prevHitTests.Any(prevHit => (prevHit.position - currentHit.position).sqrMagnitude < MinDistance))
                    {
                        prevHitTests.Add(currentHit);
                        onProgress?.Invoke((float) prevHitTests.Count / NumHitsRequired);
                        
                        _debugScanRoomHitIndicators.Add(Instantiate(_debugHitTestIndicatorPrefab, currentHit.position, currentHit.rotation));
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
        
        public void StartPlacingGroundPlane(bool preservePreviousContent = false, GameObject customPlacementIndicatorPrefab = null)
        {
            if (preservePreviousContent && _groundPlane != null)
            {
                _prevContent = Content;
            }
            else if (!preservePreviousContent)
            {
                _prevContent = null;
            }

            if (_groundPlane != null)
            {
                Destroy(_groundPlane.gameObject);
                _groundPlane = null;
            }
            
            GameObject placementIndicator = customPlacementIndicatorPrefab == null ? _defaultGroundPlanePlacementIndicatorPrefab : customPlacementIndicatorPrefab;
            CancelPlacingGroundPlane();
            _updateGroundPlanePlacementCoroutine = StartCoroutine(UpdateGroundPlanePlacement(placementIndicator));
        }
        
        public bool TryPlaceGroundPlane()
        {
            if (_placementIndicator == null)
            {
                return false;
            }

            _groundPlane = Instantiate(_groundPlanePrefab, _placementIndicator.transform.position, _placementIndicator.transform.rotation);
            if (_prevContent != null)
            {
                ParentToGround(_prevContent, false);
                _prevContent.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
            
            CancelPlacingGroundPlane();
            return true;
        }
        
        public void CancelPlacingGroundPlane()
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
        }

        private IEnumerator UpdateGroundPlanePlacement(GameObject placementIndicatorPrefab)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            
            for (;;)
            {
                if (ARManager.Instance.RaycastManager.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hit = hits[0].pose;
                    Vector3 faceAwayFromUser = Vector3.ProjectOnPlane(hit.position - ARManager.Instance.ARCamera.transform.position, hit.up);
                    
                    if (_placementIndicator == null)
                    {
                        _placementIndicator = Instantiate(placementIndicatorPrefab);
                    }
                    
                    _placementIndicator.transform.SetPositionAndRotation(hit.position, Quaternion.LookRotation(faceAwayFromUser, hit.up));
                }
                
                yield return null;
            }
        }

        public bool RaycastGroundPlane(Vector2 normalizedScreenPosition, out RaycastHit hit)
        {
            if (_groundPlane == null)
            {
                throw new InvalidOperationException("Ground plane does not exist");
            }

            Ray ray = ARManager.Instance.ARCamera.ScreenPointToRay(new Vector2(Screen.width * normalizedScreenPosition.x, Screen.height * normalizedScreenPosition.y));
            return Physics.Raycast(ray, out hit, LayerMask.GetMask(GroundPlane.Layer));
        }
        
        public GameObject GroundedInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (_groundPlane == null)
            {
                throw new InvalidOperationException("Ground plane does not exist");
            }
            
            return Instantiate(prefab, position, rotation, _groundPlane.Content);   
        }
        
        public GameObject GroundedInstantiate(GameObject prefab, bool instantiateInWorldSpace = false)
        {
            if (_groundPlane == null)
            {
                throw new InvalidOperationException("Ground plane does not exist");
            }
            
            return Instantiate(prefab, _groundPlane.Content, instantiateInWorldSpace);
        }

        public void ParentToGround(Transform t, bool worldPositionStays)
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
            CancelPlacingGroundPlane();
        }
    }
}
