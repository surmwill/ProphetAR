using System;
using UnityEngine;

namespace ProphetAR
{
    [Serializable]
    public class GridSnap
    {
        [SerializeField]
        private GridCell _origin = null;

        [SerializeField]
        private GridCell _snap = null;

        [SerializeField]
        [ReadOnly]
        private GridSection _snapSection = null;

        [SerializeField]
        private GridDirection _snapDirection = default;

        public GridCell Origin => _origin;

        public GridCell Snap => _snap;

        public bool IsValid => Origin != null && Snap != null;

        public void SetOrigin(GridCell origin)
        {
            _origin = origin;
        }
        
        public void SetSnap(GridCell snap)
        {
            _snap = snap;
        }

        public void SetSnapSection(GridSection snapSection)
        {
            _snapSection = snapSection;
        }

        // Snaps the snap cells to the origin cells. Origin cells do not move
        public void SnapTogether()
        {
            if (_origin.GridSection == _snap.GridSection)
            {
                Debug.LogError("Cannot snap a grid section to itself");
                return;
            }

            if (_origin.GridSection.CellDimensions != _snap.GridSection.CellDimensions)
            {
                Debug.LogError("Grid sections must have the same dimensions to snap together");
                return;
            }
            
            Vector2 cellDimensions = _origin.GridSection.CellDimensions;
            Vector3 snapToPosition = default;
            Vector3 offset = default;
            
            switch (_snapDirection)
            {
                case GridDirection.Left:
                    snapToPosition = _origin.transform.parent.TransformPoint(_origin.transform.localPosition.AddX(-cellDimensions.x));
                    break;
                
                case GridDirection.Right:
                    snapToPosition = _origin.transform.parent.TransformPoint(_origin.transform.localPosition.AddX(cellDimensions.x));
                    break;
                
                case GridDirection.Up:
                    snapToPosition = _origin.transform.parent.TransformPoint(_origin.transform.localPosition.AddZ(cellDimensions.y));
                    break;
                
                case GridDirection.Down:
                    snapToPosition = _origin.transform.parent.TransformPoint(_origin.transform.localPosition.AddZ(-cellDimensions.y));
                    break;
            }
            
            offset = snapToPosition - _snap.transform.position;
            _snap.GridSection.transform.position += offset;
        }
    }
}