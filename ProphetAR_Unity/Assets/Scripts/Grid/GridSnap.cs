using System;
using UnityEngine;

namespace ProphetAR
{
    [Serializable]
    public class GridSnap
    {
        [SerializeField]
        private GridCell _snap = null;

        [SerializeField]
        private GridCell _snapTo = null;

        [SerializeField]
        [ReadOnly]
        private GridSection _snapToSection = null;

        [SerializeField]
        private GridDirection _snapDirection = default;

        public GridCell Snap => _snap;

        public GridCell SnapTo => _snapTo;

        public void SetSnap(GridCell snap)
        {
            _snap = snap;
        }
        
        public void SetSnapTo(GridCell snapTo)
        {
            _snapTo = snapTo;
        }
        
        public void SetSnapToSection(GridSection snapToSection)
        {
            _snapToSection = snapToSection;
        }

        public void SnapTogether()
        {
            if (_snap.ParentGridSection == _snapTo.ParentGridSection)
            {
                Debug.LogError("Cannot snap a grid section to itself");
                return;
            }

            if (_snap.ParentGridSection.CellDimensions != _snapTo.ParentGridSection.CellDimensions)
            {
                Debug.LogError("Grid sections must have the same dimensions to snap together");
                return;
            }
            
            Vector2 cellDimensions = _snapTo.ParentGridSection.CellDimensions;
            Vector3 snapToPosition = default;
            Vector3 offset = default;
            
            switch (_snapDirection)
            {
                case GridDirection.Left:
                    snapToPosition = _snapTo.transform.parent.TransformPoint(_snapTo.transform.localPosition.AddX(-cellDimensions.x));
                    break;
                
                case GridDirection.Right:
                    snapToPosition = _snapTo.transform.parent.TransformPoint(_snapTo.transform.localPosition.AddX(cellDimensions.x));
                    break;
                
                case GridDirection.Up:
                    snapToPosition = _snapTo.transform.parent.TransformPoint(_snapTo.transform.localPosition.AddZ(cellDimensions.y));
                    break;
                
                case GridDirection.Down:
                    snapToPosition = _snapTo.transform.parent.TransformPoint(_snapTo.transform.localPosition.AddZ(-cellDimensions.y));
                    break;
            }
            
            offset = snapToPosition - _snap.transform.position;
            _snap.ParentGridSection.transform.position += offset;
        }
    }
}