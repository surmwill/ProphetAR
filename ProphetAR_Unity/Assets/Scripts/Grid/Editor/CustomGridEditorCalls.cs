#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public partial class CustomGrid
    {
        public void SaveGrid()
        {
            if (_originGridSection == null || _originGridSection.SectionDimensions.x <= 0 || _originGridSection.SectionDimensions.y <= 0)
            {
                _savedGrid.Clear();
                return;
            }
            
            _originGridSection.SnapSectionsToSelf();
            
            GridCell origin = _originGridSection.GetCells().First();
            Vector2 cellDimensions = origin.Dimensions;
            
            List<SavedGridCell> savedGrid = new List<SavedGridCell>();
            if (!AddCellsFromGridSectionRecursive(_originGridSection, new HashSet<GridSection>(), new Dictionary<Vector2, GridSection>(), origin, cellDimensions, savedGrid))
            {
                return;
            }

            int? minX = null;
            int? maxX = null;
            int? minY = null;
            int? maxY = null;

            foreach (SavedGridCell savedGridCell in savedGrid)
            {
                if (!minX.HasValue || savedGridCell.Coordinates.x < minX.Value)
                {
                    minX = savedGridCell.Coordinates.x;
                }
                
                if (!maxX.HasValue || savedGridCell.Coordinates.x > maxX.Value)
                {
                    maxX = savedGridCell.Coordinates.x;
                }
                
                if (!minY.HasValue || savedGridCell.Coordinates.y < minY.Value)
                {
                    minY = savedGridCell.Coordinates.y;
                }
                
                if (!maxY.HasValue || savedGridCell.Coordinates.y > maxY.Value)
                {
                    maxY = savedGridCell.Coordinates.y;
                }
                
                savedGridCell.GridCell.GridSection.SetParentGrid(this);
                savedGridCell.GridCell.SetCoordinates(savedGridCell.Coordinates);
            }

            _gridDimensions = new Vector2Int(maxX.Value - minX.Value + 1, maxY.Value - minY.Value + 1);
            _minCoordinate = new Vector2Int(minX.Value, minY.Value);
            _maxCoordinate = new Vector2Int(maxX.Value, maxY.Value);
            _savedGrid = savedGrid;
        }

        private bool AddCellsFromGridSectionRecursive(
            GridSection currentGridSection, 
            HashSet<GridSection> handledGridSections,
            Dictionary<Vector2, GridSection> alreadyFoundCoordinates,
            GridCell origin,
            Vector2 cellDimensions,
            List<SavedGridCell> currentGrid)
        {
            if (!handledGridSections.Add(currentGridSection))
            {
                return true;
            }
            
            // Calculate the coordinates of each cell in this section
            foreach (GridCell gridCell in currentGridSection.GetCells())
            {
                Vector2Int coordinates = CalculateCellCoordinatesFromOrigin(gridCell, origin, cellDimensions);
                if (alreadyFoundCoordinates.TryGetValue(coordinates, out GridSection fromGridSection))
                {
                    Debug.LogError($"Duplicate coordinates {coordinates} found in sections `{currentGridSection.name}` and `{fromGridSection.name}`");
                    return false;
                }
                   
                alreadyFoundCoordinates.Add(coordinates, currentGridSection);
                currentGrid.Add(new SavedGridCell(gridCell, coordinates));
            }
            
            // Calculate the coordinates of each cell in snapped sections
            foreach (GridSection gridSection in currentGridSection.GridSnaps.Where(gridSnap => gridSnap.Snap != null).Select(gridSnap => gridSnap.Snap.GridSection))
            {
                if (!AddCellsFromGridSectionRecursive(gridSection, handledGridSections, alreadyFoundCoordinates, origin, cellDimensions, currentGrid))
                {
                    return false;
                }
            }

            return true;
        }
        
        private Vector2Int CalculateCellCoordinatesFromOrigin(GridCell cell, GridCell origin, Vector2 cellDimensions)
        {
            return new Vector2Int(
                Mathf.RoundToInt(-(cell.transform.position.z - origin.transform.position.z) / cellDimensions.y),
                Mathf.RoundToInt((cell.transform.position.x - origin.transform.position.x) / cellDimensions.x));
        }
    }
}

#endif