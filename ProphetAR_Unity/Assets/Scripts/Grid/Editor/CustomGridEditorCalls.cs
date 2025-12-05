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

            foreach (SavedGridCell savedGridCell in savedGrid)
            {
                savedGridCell.GridCell.SetCoordinates(savedGridCell.Coordinates);
                savedGridCell.GridCell.GridSection.SetParentGrid(this);
            }
            
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
                Vector2 coordinates = CalculateCellCoordinatesFromOrigin(gridCell, origin, cellDimensions);
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
        
        private Vector2 CalculateCellCoordinatesFromOrigin(GridCell cell, GridCell origin, Vector2 cellDimensions)
        {
            return new Vector2(
                Mathf.RoundToInt(-(cell.transform.position.z - origin.transform.position.z) / cellDimensions.y),
                Mathf.RoundToInt((cell.transform.position.x - origin.transform.position.x) / cellDimensions.x));
        }
    }
}

#endif