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

        public delegate void OnHovered(T lastHover, T currHover);
        
        public T LastHovered { get; private set; }

        private static readonly bool SelectingUnityObject = typeof(Object).IsAssignableFrom(typeof(T));
        
        // Not that T could also be an interface type and not a (unity) Object
        private Object _lastHoveredUnityObject;

        private Camera _arCamera;
        
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
        /// <param name="debugDrawRays"> Draws the raycast and prints the number of hits </param>
        /// <returns> A yield instruction that yields while we're still in the selection process </returns>
        public WaitForARObjectSelection<T> StartObjectSelection(
            OnHovered onHovered = null,
            Action<T> onSelected = null, 
            Action onCancelled = null,
            Func<Transform, T> getObjectFromCollision = null,
            Func<T, bool> isValidObject = null,
            bool debugDrawRays = false)
        {
            if (_gridCellSelectionCoroutine != null)
            {
                Debug.LogWarning($"Existing AR object selection of type {nameof(T)} cancelled. Only one can run at a time.");
                Cancel();
            }
            
            _onSelected = onSelected;
            _onCancelled = onCancelled;

            _waitForObjectSelection = new WaitForARObjectSelection<T>(this);
            _gridCellSelectionCoroutine = StartCoroutine(GridCellSelection(onHovered, getObjectFromCollision, isValidObject, debugDrawRays));

            return _waitForObjectSelection;
        }

        private IEnumerator GridCellSelection(
            OnHovered onHovered = null, 
            Func<Transform, T> getObjectFromCollision = null, 
            Func<T, bool> isValidObject = null,
            bool debugDrawRays = false)
        {
            for (;;)
            {
                Ray cameraRay = new Ray(_arCamera.transform.position, _arCamera.transform.forward);
                int numHits = Physics.RaycastNonAlloc(cameraRay, _raycastHits, Mathf.Infinity, _gridObjectLayers);

                if (debugDrawRays)
                {
                    Debug.DrawRay(_arCamera.transform.position, _arCamera.transform.forward * 10, Color.green, 10f);   
                    Debug.Log($"Num hits for {nameof(ARObjectSelector<T>)}: {numHits}");
                }
                
                Object closestValidUnityObject = null;
                T closestValidObject = null;
                float? closestValidDistance = null;
                
                for (int i = 0; i < numHits; i++)
                {
                    RaycastHit raycastHit = _raycastHits[i];
                    Transform hitTransform = raycastHit.transform;
                    float hitDistance = raycastHit.distance;
                    
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
            if (_gridCellSelectionCoroutine == null)
            {
                return;
            }
            
            StopCoroutine(_gridCellSelectionCoroutine);
            _gridCellSelectionCoroutine = null;

            T selected = LastHovered;
            LastHovered = null;
            
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

            if (!fromSelectionSuccess)
            {
                onCancelled?.Invoke();   
            }
        }
    }
}