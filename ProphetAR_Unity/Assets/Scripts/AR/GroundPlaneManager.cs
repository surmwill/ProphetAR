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
        private GameObject _anchorPrefab = null;

        [SerializeField]
        private GroundPlane _groundPlanePrefab = null;

        [SerializeField]
        private GameObject _defaultPlacementIndicator = null;
        
        [Header("Debug")]
        [SerializeField]
        private GameObject _debugHitTestIndicatorPrefab = null;
        
        public GroundPlane Plane { get; private set; }

        private const float ScanRoomHitTestInterval = 0.1f;

        private Coroutine _scanRoomCoroutine;

        private Transform _prevGroundPlane;

        public void RecalculateGroundPlane(GameObject oldGroundPlaneContent)
        {
            if (_scanRoomCoroutine != null)
            {
                StopCoroutine(_scanRoomCoroutine);
            }
            _scanRoomCoroutine = StartCoroutine(ScanRoom());
        }

        public void PlaceGroundPlane(bool preservePreviousContent, GameObject placementIndicatorPrefab = null)
        {
            Transform prevContent = null;
            if (preservePreviousContent && Plane != null)
            {
                prevContent = Plane.Content;
            }

            if (Plane != null)
            {
                Destroy(Plane.gameObject);   
            }
            
            
        }

        private IEnumerator ScanRoom()
        {
            const float Interval = 0.1f;
            const float MinDistance = 0.1f;
            const float NumHitTests = 20;

            List<Pose> prevHitTests = new List<Pose>();
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            while (prevHitTests.Count < NumHitTests)
            {
                if (ARManager.Instance.RaycastManager.Raycast(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f), hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose currentHit = hits[0].pose;
                    if (!prevHitTests.Any(prevHit => (prevHit.position - currentHit.position).sqrMagnitude < MinDistance))
                    {
                        prevHitTests.Add(currentHit);
                    }
                }

                yield return new WaitForSeconds(Interval);
            }

            _scanRoomCoroutine = null;
        }
        
        public GameObject GroundedInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Instantiate(prefab, position, rotation, Plane.Content);   
        }
        
        public GameObject GroundedInstantiate(GameObject prefab, bool instantiateInWorldSpace = false)
        {
            return Instantiate(prefab, Plane.Content, instantiateInWorldSpace);
        }

        public void ParentToGround(GameObject go, bool worldPositionStays)
        {
            go.transform.SetParent(Plane.Content, worldPositionStays);
        }
        
        private void OnDestroy()
        {
            if (_scanRoomCoroutine != null)
            {
                StopCoroutine(_scanRoomCoroutine);
            }
        }
    }
}
