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
        
        public T LastHovered { get; private set; }

        private static readonly bool SelectingUnityObject = typeof(Object).IsAssignableFrom(typeof(T));
        
        // Not that T could also be an interface type and not a (unity) Object
        private Object _lastHoveredUnityObject;

        private Camera _arCamera;
        
        private Coroutine _gridCellSelectionCoroutine;
        private ARSelectingObjectYieldInstruction _selectingObjectYieldInstruction;
        
        private Action<T> _onSelected;
        private Action _onCancelled;
        
        // The length is an estimate on the upper bound of the number of hits we'll receive in a raycast (we might then filter out invalid ones)
        private readonly RaycastHit[] _raycastHits = new RaycastHit[5];

        public void Initialize(Camera arCamera)
        {
            _arCamera = arCamera;
        }
        
        public ARSelectingObjectYieldInstruction StartObjectSelection(
            Action<T> onHovered = null, 
            Action<T> onSelected = null, 
            Action onCancelled = null,
            Func<T, bool> isValidForSelection = null)
        {
            if (_gridCellSelectionCoroutine != null)
            {
                Debug.LogWarning($"Existing AR object selection of type {nameof(T)} cancelled. Only one can run at a time.");
                Cancel();
            }
            
            _onSelected = onSelected;
            _onCancelled = onCancelled;

            _selectingObjectYieldInstruction = new ARSelectingObjectYieldInstruction();
            _gridCellSelectionCoroutine = StartCoroutine(GridCellSelection(onHovered, isValidForSelection));

            return _selectingObjectYieldInstruction;
        }

        private IEnumerator GridCellSelection(Action<T> onHovered = null, Func<T, bool> isValidForSelection = null)
        {
            for (;;)
            {
                Ray cameraRay = new Ray(_arCamera.transform.position, _arCamera.transform.forward);
                int numHits = Physics.RaycastNonAlloc(cameraRay, _raycastHits, Mathf.Infinity, _gridObjectLayers);
                
                for (int i = 0; i < numHits; i++)
                {
                    T hitGridObject = _raycastHits[i].transform.GetComponent<T>();
                    bool cachedIsValidForSelection = false;
                    
                    // Component (overrides ==)
                    if (SelectingUnityObject)
                    {
                        if (hitGridObject is Object unityObject && 
                            unityObject != null && 
                            unityObject != _lastHoveredUnityObject &&
                            (isValidForSelection?.Invoke(hitGridObject) ?? true))
                        {
                            _lastHoveredUnityObject = unityObject;
                            LastHovered = hitGridObject;
                            onHovered?.Invoke(hitGridObject);
                            break;
                        }
                    }
                    // Interface
                    else if (hitGridObject != null && LastHovered != hitGridObject &&  (isValidForSelection?.Invoke(hitGridObject) ?? true))
                    {
                        LastHovered = hitGridObject;
                        onHovered?.Invoke(hitGridObject);
                        break;
                    }
                }

                yield return null;
            }
        }

        public T Select(bool keepSelecting = false)
        {
            _onSelected?.Invoke(LastHovered);
            T selected = LastHovered;

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

        private void CancelInner(bool fromSelection)
        {
            if (_gridCellSelectionCoroutine == null)
            {
                return;
            }
            
            LastHovered = null;
            
            StopCoroutine(_gridCellSelectionCoroutine);
            _gridCellSelectionCoroutine = null;

            if (fromSelection)
            {
                _selectingObjectYieldInstruction.Selected = true;
            }
            else
            {
                _selectingObjectYieldInstruction.Cancelled = true;
            }
            _selectingObjectYieldInstruction = null;

            Action onCancelled = _onCancelled;
            _onCancelled = null;
            _onSelected = null;

            if (!fromSelection)
            {
                onCancelled?.Invoke();   
            }
        }
    }
}