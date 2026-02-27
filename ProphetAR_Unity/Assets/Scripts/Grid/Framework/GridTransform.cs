using System;
using System.Collections;
using GridOperations;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Contains a position on the grid and the ability to move around to other positions
    /// </summary>
    public class GridTransform : MonoBehaviour
    {
        public delegate IEnumerator AnimateMovement(Transform parent, Vector3 localPosition, float time);
        
        public GridObject GridObject { get; private set; }
        
        public CustomGrid Grid { get; private set; }

        public GridCellContent CurrentCell => Grid[Coordinates].Content;

        public IGridContentSelfPositioner CurrentCellPositioner
        {
            get
            {
                if (_checkedForCurrentCellPositioner)
                {
                    return _currentCellPositioner;
                }

                _checkedForCurrentCellPositioner = true;
                _currentCellPositioner = GetComponentInChildren<IGridContentSelfPositioner>();

                return _currentCellPositioner;
            }
        }

        private bool _checkedForCurrentCellPositioner;
        private IGridContentSelfPositioner _currentCellPositioner;
        
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
        
        public IEnumerator MoveToAnimated(Vector2Int moveToCoordinates, AnimateMovement animateMovement, float time)
        {
            if (!HasCoordinates)
            {
                throw new InvalidOperationException("GridTransform is missing coordinates to start the move from");
            }
            
            if (Coordinates == moveToCoordinates)
            {
                Debug.LogWarning("Moving to the same coordinates we're already on");
                yield break;
            }
            
            GridCell gridCell = Grid[moveToCoordinates];
            if (gridCell == null)
            {
                throw new ArgumentException($"Coordinates not in grid {moveToCoordinates}");
            }

            GridCellContent gridCellContent = gridCell.Content;
            Transform toParent = CurrentCellPositioner == null ? gridCellContent.transform : CurrentCellPositioner.GetCellParent(gridCellContent);
            Vector3 toLocalPosition = CurrentCellPositioner == null ? Vector3.zero : CurrentCellPositioner.GetLocalPositionInCell(gridCellContent);
            
            yield return animateMovement(toParent, toLocalPosition, time);
            
            MoveToImmediate(moveToCoordinates);
        }

        public void MoveToImmediate(Vector2Int moveToCoordinates)
        {
            if (HasCoordinates)
            {
                if (Coordinates == moveToCoordinates)
                {
                    Debug.LogWarning("Moving to the same coordinates we're already on");
                    return;
                }
                
                Grid[Coordinates].Content.RemoveOccupier(this);
            }
            
            GridCell gridCell = Grid[moveToCoordinates];
            if (gridCell == null)
            {
                throw new ArgumentException($"Coordinates not in grid {moveToCoordinates}");
            }

            GridCellContent gridCellContent = gridCell.Content;
            Transform toParent = CurrentCellPositioner == null ? gridCellContent.transform : CurrentCellPositioner.GetCellParent(gridCellContent);
            Vector3 toLocalPosition = CurrentCellPositioner == null ? Vector3.zero : CurrentCellPositioner.GetLocalPositionInCell(gridCellContent);
            
            transform.SetParent(toParent);
            transform.localPosition = toLocalPosition;
            Coordinates = moveToCoordinates;
            
            gridCellContent.AddOccupier(this);
        }

        public GridSliceNavigationDestinations GetPathsFrom(int maxNumSteps, GridSlice area)
        {
            if (!area.ContainsCoordinates(Coordinates))
            {
                throw new ArgumentException($"The transform at {Coordinates} is not contained in the grid slice with properties: (${area.PrintSliceDescription()})");
            }

            SerializedGrid serializedGrid = area.GetSerializedGrid().WithOrigin((Coordinates - area.TopLeft).ToTuple());
            return new GridSliceNavigationDestinations(area, GridPathFinder.GetPathsFrom(serializedGrid, maxNumSteps, true));
        }
        
        public GridSliceNavigationInstructions GetPathTo(Vector2Int target, GridSlice area)
        {
            if (!area.ContainsCoordinates(Coordinates))
            {
                throw new ArgumentException($"The transform at {Coordinates} is not contained in the grid slice with properties: (${area.PrintSliceDescription()})");
            }
            
            if (!area.ContainsCoordinates(target))
            {
                throw new ArgumentException($"The target {target} is not contained in the grid slice with properties: (${area.PrintSliceDescription()})");
            }

            SerializedGrid serializedGrid = area.GetSerializedGrid()
                .WithOrigin((Coordinates - area.TopLeft).ToTuple())
                .WithTarget(target.ToTuple());

            return new GridSliceNavigationInstructions(area, GridPathFinder.GetPathTo(serializedGrid));
        }
    }
}