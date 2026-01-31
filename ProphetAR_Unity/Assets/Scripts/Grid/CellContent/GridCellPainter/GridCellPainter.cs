using System;
using TMPro;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Paints overlays (ex: green if the cell is navigable, red for being in attack range)
    /// </summary>
    [ExecuteAlways]
    public class GridCellPainter : MonoBehaviour, IGridCellContentDimensionsChangedListener
    {
        [SerializeField]
        private GridCellContent _gridCellContent = null;
        
        [SerializeField]
        private GameObject _navigableIndicator = null;

        [SerializeField]
        private TextMeshPro _navigableNumSteps = null;

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (_gridCellContent != null)
            {
                _gridCellContent.RegisterOnDimensionsChangedListener(this);
            }
            #endif
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (_gridCellContent != null)
            {
                _gridCellContent.UnregisterOnDimensionsChangedListener(this);   
            }
            #endif
        }

        public void ShowIsNavigable(bool show, int numSteps = 0)
        {
            if (show)
            {
                _navigableIndicator.gameObject.SetActive(true); 
                _navigableNumSteps.text = numSteps.ToString();
            }
            else
            {
                _navigableIndicator.gameObject.SetActive(false);   
            }
        }

        public void OnDimensionsChanged(Vector2 newDimensions)
        {
            transform.position = _gridCellContent.Cell.Middle;
            transform.localScale = new Vector3(newDimensions.x, 1f, newDimensions.y);
        }
    }
}