#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public partial class Grid
    {
        private bool DoGridSectionsAlign()
        {
            if (_gridSections.Count == 0)
            {
                return true;
            }

            GridSection firstSection = _gridSections[0];
            Vector2 firstCellDimensions = firstSection.CellDimensions;

            foreach (GridSection gridSection in _gridSections.Skip(1))
            {
                if (gridSection.CellDimensions != firstCellDimensions)
                {
                    Debug.LogError($"Grid section `{firstSection.name}` has different cell dimensions than `{gridSection.name}`");
                    return false;
                }
            }

            return true;
        }

        private bool AreAnySectionsEmpty()
        {
            foreach (GridSection section in _gridSections)
            {
                if (!section.GetCells().Any())
                {
                    Debug.LogError($"Grid section `{section.name}` is empty");
                    return true;
                }
            }

            return false;
        }

        public void SnapGridCellsTogether(GridCell snap, GridCell snapTo, GridDirection snapDirection)
        {
            if (snap.ParentGridSection.CellDimensions != snapTo.ParentGridSection.CellDimensions)
            {
                Debug.LogError("Cannot snap two cells of different dimensions together");
                return;
            }

            Vector2 cellDimensions = snapTo.ParentGridSection.CellDimensions;
            Vector3 snapToPosition = default;
            Vector3 offset = default;
            
            switch (snapDirection)
            {
                case GridDirection.Left:
                    snapToPosition = snapTo.transform.TransformPoint(snapTo.transform.localPosition.AddX(-cellDimensions.x));
                    break;
                
                case GridDirection.Right:
                    snapToPosition = snapTo.transform.TransformPoint(snapTo.transform.localPosition.AddX(cellDimensions.x));
                    break;
                
                case GridDirection.Up:
                    snapToPosition = snapTo.transform.TransformPoint(snapTo.transform.localPosition.AddZ(-cellDimensions.y));
                    break;
                
                case GridDirection.Down:
                    snapToPosition = snapTo.transform.TransformPoint(snapTo.transform.localPosition.AddZ(cellDimensions.y));
                    break;
            }
            
            offset = snapToPosition - snap.transform.position;
            snap.ParentGridSection.transform.position += offset;
        }
        
        public void SaveGrid()
        {
            if (_gridSections.Count == 0)
            {
                _origin = null;
                _cellDimensions = Vector2.zero;
                _savedGrid.Clear();
                
                return;
            }
            
            if (!DoGridSectionsAlign() || AreAnySectionsEmpty())
            {
                return;
            }
            
            GridSection firstSection = _gridSections[0];
            GridCell origin = firstSection.GetCells().First();
            Vector2 cellDimensions = firstSection.CellDimensions;
            
            List<SavedGridCell> savedGridCells = new List<SavedGridCell>();
            Dictionary<Vector2, GridSection> previousCoordinates = new Dictionary<Vector2, GridSection>();
            
            foreach (GridSection gridSection in _gridSections)
            {
                gridSection.SetParentGrid(this);
                
                foreach (GridCell gridCell in gridSection.GetCells())
                {
                    Vector2 coordinates = CalculateCellCoordinatesFromOrigin(gridCell, origin, cellDimensions);
                    if (previousCoordinates.TryGetValue(coordinates, out GridSection fromGridSection))
                    {
                        Debug.LogError($"Duplicate coordinates {coordinates} found in sections `{gridSection.name}` and `{fromGridSection.name}`");
                        return;
                    }
                   
                    previousCoordinates.Add(coordinates, gridSection);
                    savedGridCells.Add(new SavedGridCell(gridCell, coordinates));
                }
            }

            foreach (SavedGridCell savedGridCell in savedGridCells)
            {
                savedGridCell.GridCell.SetCoordinates(savedGridCell.Coordinates);
            }

            _origin = origin;
            _cellDimensions = cellDimensions;
            _savedGrid = savedGridCells;
        }
        
        private Vector2 CalculateCellCoordinatesFromOrigin(GridCell cell, GridCell origin, Vector2 cellDimensions)
        {
            return new Vector2(
                Mathf.RoundToInt((cell.transform.position.x - origin.transform.position.x) / cellDimensions.x),
                Mathf.RoundToInt((cell.transform.position.z - origin.transform.position.z) * -1 / cellDimensions.y));
        }
    }
}

#endif