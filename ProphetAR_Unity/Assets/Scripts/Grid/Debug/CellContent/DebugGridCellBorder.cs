using UnityEngine;
using UnityEngine.UI;

namespace ProphetAR
{
    [ExecuteAlways]
    public class DebugGridCellBorder : MonoBehaviour, IGridCellContentDimensionsChangedListener, IGridCellContentCoordinatesChangedListener
    {
        [SerializeField]
        private GridCellContent _gridCellContent = null;
        
        [SerializeField]
        private Image _borderImage = null;

        private const float ScaleDownForMargin = 0.95f;

        private bool _areEditModeListenersBound;

        private void OnEnable()
        {
            #if UNITY_EDITOR
            _gridCellContent.RegisterOnCoordinatesChangedListener(this);
            _gridCellContent.RegisterOnDimensionsChangedListener(this);
            #endif
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            _gridCellContent.UnregisterOnCoordinatesChangedListener(this);
            _gridCellContent.UnregisterOnCoordinatesChangedListener(this);
            #endif
        }

        public void OnDimensionsChanged(Vector2 newDimensions)
        {
            transform.localScale = new Vector3(ScaleDownForMargin * newDimensions.x, 1f, ScaleDownForMargin * newDimensions.y);
        }

        public void OnCoordinatesChanged(Vector2Int newCoordinates)
        {
            _borderImage.color = newCoordinates == Vector2.zero ? Color.green : Color.white;
        }
    }
}
