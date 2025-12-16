using System;
using System.Collections;
using GridPathFinding;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Contains a position on the grid and the ability to move around to other positions
    /// </summary>
    public class GridTransform : MonoBehaviour
    {
        public GridObject GridObject { get; private set; }
        
        public CustomGrid Grid { get; private set; }
        
        public Vector2Int Coordinates {
            get
            {
                if (!HasCoordinates)
                {
                    throw new InvalidOperationException($"No initial coordinates have been given. Please set them directly or with {nameof(MoveToImmediate)}");
                }

                return _coordinates.Value;

            }
            set
            {
                if (Grid == null)
                {
                    throw new InvalidOperationException("Uninitialized");
                }
                
                if (Grid[value] == null)
                {
                    throw new InvalidOperationException($"Coordinates {value} are not contained within the grid");
                }
                
                _coordinates = value;  
            } 
        }
        
        public bool HasCoordinates => _coordinates.HasValue;

        private Vector2Int? _coordinates = null;

        public void Initialize(GridObject gridObject, CustomGrid grid)
        {
            GridObject = gridObject;
            Grid = grid;
        }

        public IGridCellCorners GetCellCorners(Vector2Int coordinates)
        {
            GridCell gridCell = Grid[coordinates];
            if (gridCell == null)
            {
                throw new ArgumentException($"Coordinates not in grid {coordinates}");
            }

            return gridCell;
        }
        
        public IEnumerator MoveToAnimated(Vector2Int moveToCoordinates, Func<Transform, IEnumerator> animateMovement, Func<GridCellContent, Transform> getCustomParentInCell = null)
        {
            if (!HasCoordinates)
            {
                throw new InvalidOperationException("GridTransform is missing coordinates to start the move from");
            }
            
            GridCell gridCell = Grid[moveToCoordinates];
            if (gridCell == null)
            {
                throw new ArgumentException($"Coordinates not in grid {moveToCoordinates}");
            }

            GridCellContent gridCellContent = gridCell.Content;
            Transform parent = getCustomParentInCell == null ? gridCellContent.transform : getCustomParentInCell(gridCellContent);
            
            yield return animateMovement(parent);
            
            MoveToImmediate(moveToCoordinates, getCustomParentInCell);
        }

        public void MoveToImmediate(Vector2Int moveToCoordinates, Func<GridCellContent, Transform> getCustomParentInCell = null)
        {
            if (HasCoordinates)
            {
                Grid[Coordinates].Content.RemoveOccupier(this);
            }
            
            GridCell gridCell = Grid[moveToCoordinates];
            if (gridCell == null)
            {
                throw new ArgumentException($"Coordinates not in grid {moveToCoordinates}");
            }

            GridCellContent gridCellContent = gridCell.Content;
            Transform parent = getCustomParentInCell == null ? gridCellContent.transform : getCustomParentInCell(gridCellContent);
            
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            Coordinates = moveToCoordinates;
            
            gridCellContent.AddOccupier(this);
        }

        public NavigationDestinationSet GetPathsFrom(int maxNumSteps, GridSlice area)
        {
            if (!area.ContainsPoint(Coordinates))
            {
                throw new ArgumentException($"The transform at {Coordinates} is not contained in the grid slice with properties: (${area.SliceDescription()})");
            }

            SerializedGrid serializedGrid = area.GetSerializedGrid().WithOrigin(Coordinates.ToTuple());
            return GridPathFinder.GetPathsFrom(serializedGrid, maxNumSteps);
        }
        
        public NavigationInstructionSet GetPathTo(Vector2Int target, GridSlice area)
        {
            if (!area.ContainsPoint(Coordinates))
            {
                throw new ArgumentException($"The transform at {Coordinates} is not contained in the grid slice with properties: (${area.SliceDescription()})");
            }
            
            if (!area.ContainsPoint(target))
            {
                throw new ArgumentException($"The target {target} is not contained in the grid slice with properties: (${area.SliceDescription()})");
            }

            SerializedGrid serializedGrid = area.GetSerializedGrid()
                .WithOrigin(Coordinates.ToTuple())
                .WithTarget(target.ToTuple());

            return GridPathFinder.GetPathTo(serializedGrid);
        }
    }
}