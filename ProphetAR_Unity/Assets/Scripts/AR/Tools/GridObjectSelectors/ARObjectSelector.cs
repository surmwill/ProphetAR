using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProphetAR
{
    public class ARObjectSelector<T> : MonoBehaviour where T : class
    {
        [SerializeField]
        private LayerMask _gridObjectLayers = default;
        
        public T LastHovered { get; private set; }

        // Not that T could also be an interface type and not a (unity) Object
        private Object _lastHoveredUnityObject;

        private Camera _arCamera;
        
        private Coroutine _gridCellSelectionCoroutine;
        private ARSelectingObjectYieldInstruction _selectingObjectYieldInstruction;
        
        private Action<T> _onSelected;
        private Action _onCancelled;

        public void Initialize(Camera arCamera)
        {
            _arCamera = arCamera;
        }
        
        public ARSelectingObjectYieldInstruction StartGridCellSelection(Action<T> onHovered = null, Action<T> onSelected = null, Action onCancelled = null)
        {
            if (_gridCellSelectionCoroutine != null)
            {
                Debug.LogWarning($"Existing AR object selection of type {nameof(T)} cancelled. Only one can run at a time.");
                Cancel();
            }
            
            _onSelected = onSelected;
            _onCancelled = onCancelled;

            _selectingObjectYieldInstruction = new ARSelectingObjectYieldInstruction();
            _gridCellSelectionCoroutine = StartCoroutine(GridCellSelection(onHovered));

            return _selectingObjectYieldInstruction;
        }

        private IEnumerator GridCellSelection(Action<T> onHovered = null)
        {
            for (;;)
            {
                Ray cameraRay = new Ray(_arCamera.transform.position, _arCamera.transform.forward);
                if (Physics.Raycast(cameraRay, out RaycastHit raycastHit, Mathf.Infinity, _gridObjectLayers))
                {
                    T hitGridObject = raycastHit.transform.GetComponent<T>();
                    
                    // Component
                    if (hitGridObject is Object unityObject)
                    {
                        if (unityObject != null && unityObject != _lastHoveredUnityObject)
                        {
                            _lastHoveredUnityObject = unityObject;
                            LastHovered = hitGridObject;
                            onHovered?.Invoke(hitGridObject);
                        }
                    }
                    // Interface
                    else if (hitGridObject != null && LastHovered != hitGridObject)
                    {
                        LastHovered = hitGridObject;
                        onHovered?.Invoke(hitGridObject);
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