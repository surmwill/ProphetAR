using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProphetAR
{
    public abstract class ARObjectSelector<T> : MonoBehaviour where T : class
    {
        [SerializeField]
        private LayerMask _gridObjectLayers = default;

        [SerializeField]
        private ARRaycastDrawer _defaultRaycastDrawerPrefab = null;

        public delegate void OnHovered(T lastHover, T currHover);
        
        public T LastHovered { get; private set; }

        private static readonly bool SelectingUnityObject = typeof(Object).IsAssignableFrom(typeof(T));
        
        // Not that T could also be an interface type and not a (unity) Object
        private Object _lastHoveredUnityObject;

        private Camera _arCamera;
        
        private ARRaycastDrawer _raycastDrawer;
        
        private Coroutine _gridCellSelectionCoroutine;
        private WaitForARObjectSelection<T> _waitForObjectSelection;
        
        private Action<T> _onSelected;
        private Action _onCancelled;
        
        // The length is an estimate on the upper bound of the number of hits we'll receive in a raycast (we might then filter out invalid ones)
        private readonly RaycastHit[] _raycastHits = new RaycastHit[5];

        public void Initialize(Camera arCamera)
        {
            _arCamera = arCamera;
        }

        /// <summary>
        /// Begins ray-casting from the AR camera looking for the closest valid object of the supplied type.
        /// The user can select the object, continue looking for another object, or cancel the process.
        /// </summary>
        /// <param name="onHovered"> Callback for when we've raycasted against a new closest valid object </param>
        /// <param name="onSelected"> Callback for when we've selected an object </param>
        /// <param name="onCancelled"> Callback if we cancelled the selection process </param>
        /// <param name="getObjectFromCollision"> Gets the object from the collision transform (default is GetComponent) </param>
        /// <param name="isValidObject"> Returns true if the object is valid for selection (default true) </param>
        /// <param name="drawRaycasts"> Whether to draw the raycast </param>
        /// <param name="customRaycastDrawer"> How to draw the raycast </param>
        /// <returns> A yield instruction that yields while we're still in the selection process </returns>
        public WaitForARObjectSelection<T> StartObjectSelection(
            OnHovered onHovered = null,
            Action<T> onSelected = null,
            Action onCancelled = null,
            Func<Transform, T> getObjectFromCollision = null,
            Func<T, bool> isValidObject = null,
            bool drawRaycasts = true,
            ARRaycastDrawer customRaycastDrawer = null,
            bool outputDebug = false)
        {
            if (_gridCellSelectionCoroutine != null)
            {
                Debug.LogWarning($"Existing AR object selection of type {nameof(T)} cancelled. Only one can run at a time.");
                Cancel();
            }
            
            _onSelected = onSelected;
            _onCancelled = onCancelled;

            _waitForObjectSelection = new WaitForARObjectSelection<T>(this);

            ARRaycastDrawer raycastDrawer = null;
            if (drawRaycasts)
            {
                raycastDrawer = customRaycastDrawer != null ? customRaycastDrawer : _defaultRaycastDrawerPrefab;
            }
            
            _gridCellSelectionCoroutine = StartCoroutine(GridCellSelection(onHovered, getObjectFromCollision, isValidObject, raycastDrawer, outputDebug));
            return _waitForObjectSelection;
        }

        private IEnumerator GridCellSelection(
            OnHovered onHovered = null, 
            Func<Transform, T> getObjectFromCollision = null, 
            Func<T, bool> isValidObject = null,
            ARRaycastDrawer raycastDrawerPrefab = null,
            bool outputDebug = false)
        {
            // Wait one frame to return the custom yield instruction 
            yield return null;
            
            for (;;)
            {
                Ray cameraRay = new Ray(_arCamera.transform.position, _arCamera.transform.forward);
                int numHits = Physics.RaycastNonAlloc(cameraRay, _raycastHits, Mathf.Infinity, _gridObjectLayers);

                if (outputDebug)
                {
                    Debug.Log($"Num hits for {nameof(ARObjectSelector<T>)}: {numHits}");   
                }
                
                Object closestValidUnityObject = null;
                T closestValidObject = null;
                float? closestValidDistance = null;

                if (numHits == 0 && _raycastDrawer != null)
                {
                    _raycastDrawer.gameObject.SetActive(false);
                }
                
                for (int i = 0; i < numHits; i++)
                {
                    RaycastHit raycastHit = _raycastHits[i];
                    Transform hitTransform = raycastHit.transform;
                    float hitDistance = raycastHit.distance;
                    
                    if (raycastDrawerPrefab != null)
                    {
                        if (_raycastDrawer == null)
                        {
                            _raycastDrawer = Instantiate(raycastDrawerPrefab);   
                        }
                    
                        _raycastDrawer.gameObject.SetActive(true);
                        _raycastDrawer.DrawRaycast(raycastHit);
                    }
                    
                    T hitGridObject = getObjectFromCollision?.Invoke(hitTransform) ?? hitTransform.GetComponent<T>();
                    
                    // Component (overriden ==)
                    if (SelectingUnityObject)
                    {
                        if (hitGridObject is Object unityObject && 
                            unityObject != null && 
                            (!closestValidDistance.HasValue || hitDistance < closestValidDistance.Value) &&
                            (isValidObject?.Invoke(hitGridObject) ?? true))
                        {
                            closestValidUnityObject = unityObject;
                            closestValidObject = hitGridObject;
                            closestValidDistance = hitDistance;
                        }
                    }
                    // Interface
                    else if (hitGridObject != null && 
                             LastHovered != hitGridObject && 
                             (!closestValidDistance.HasValue || hitDistance < closestValidDistance.Value) &&
                             (isValidObject?.Invoke(hitGridObject) ?? true))
                    {

                        closestValidObject = hitGridObject;
                        closestValidDistance = hitDistance;
                        break;
                    }
                }
                
                if (closestValidDistance.HasValue &&
                    (SelectingUnityObject && closestValidUnityObject != _lastHoveredUnityObject || (!SelectingUnityObject && LastHovered != closestValidObject)))
                {
                    T prevHover = LastHovered;
                    
                    LastHovered = closestValidObject;
                    if (SelectingUnityObject)
                    {
                        _lastHoveredUnityObject = closestValidUnityObject;
                    }
                    
                    onHovered?.Invoke(prevHover, LastHovered);
                }

                yield return null;
            }
        }

        public T Select(bool keepSelecting = false)
        {
            if ((SelectingUnityObject && _lastHoveredUnityObject == null) || (!SelectingUnityObject && LastHovered == null))
            {
                throw new InvalidOperationException("Nothing to select (last hover is null)");
            }
            
            T selected = LastHovered;
            _onSelected?.Invoke(selected);

            if (!keepSelecting)
            {
                CancelInner(true);
            }
            
            return selected;
        }

        public void Cancel()
        {
            CancelInner(false);
        }

        private void CancelInner(bool fromSelectionSuccess)
        {
            // Previously cancelled
            if (_gridCellSelectionCoroutine == null)
            {
                return;
            }
            
            // Reset state
            StopCoroutine(_gridCellSelectionCoroutine);
            _gridCellSelectionCoroutine = null;

            if (_raycastDrawer != null)
            {
                Destroy(_raycastDrawer.gameObject);
                _raycastDrawer = null;
            }
            
            T selected = LastHovered;
            LastHovered = null;
            _lastHoveredUnityObject = null;
            
            if (fromSelectionSuccess)
            {
                _waitForObjectSelection.SetResolvedSelected(selected);
            }
            else
            {
                _waitForObjectSelection.SetResolvedCancelled();
            }
            _waitForObjectSelection = null;

            Action onCancelled = _onCancelled;
            _onCancelled = null;
            _onSelected = null;

            // Callback
            if (!fromSelectionSuccess)
            {
                onCancelled?.Invoke();   
            }
        }
    }
}