using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// The grid in the Unity scene. Called CustomGrid because Unity already has a Grid component
    /// </summary>
    public partial class CustomGrid : MonoBehaviour, ISerializationCallbackReceiver, IEnumerable<GridCell>
    {
        [SerializeField]
        private Level _level = null;
        
        [Tooltip("(rows, columns)")]
        [SerializeField]
        [ReadOnly]
        private Vector2Int _gridDimensions = default;

        [SerializeField]
        [ReadOnly]
        private Vector2Int _topLeftCoordinate = default;
        
        [SerializeField]
        [ReadOnly]
        private Vector2Int _botRightCoordinate = default;
        
        [Tooltip("Used for building the grid")]
        [SerializeField]
        private GridSection _originGridSection = null;

        [SerializeField]
        private bool _showGridPointTypeIndicators = false;
        
        [SerializeField]
        private bool _showSpawnPoints = false;
        
        [SerializeField]
        [ReadOnly]
        private List<SavedGridCell> _savedGrid = null;

        public event Action<GridObject> OnAddedGridObject;

        public event Action<GridObject, bool> OnRemovedGridObject;
        
        public Level Level => _level;
        
        public Dictionary<Vector2Int, GridCell> Cells { get; } = new();
        
        public GridPainter GridPainter { get; private set; }
        
        private readonly HashSet<GridObject> _gridObjects = new();
        
        private void Awake()
        {
            GridPainter = new GridPainter(this);
        }
        
        // Query the structure of the built grid
        #region GridQuerying
        
        public GridCell this[Vector2Int coordinates] => Cells.GetValueOrDefault(coordinates);

        public Vector2Int ClampCoordinate(Vector2Int coordinate)
        {
            return new Vector2Int(
                Mathf.Clamp(coordinate.x, _topLeftCoordinate.x, _botRightCoordinate.x),
                Mathf.Clamp(coordinate.y, _topLeftCoordinate.y, _botRightCoordinate.y));
        }
        
        public GridSlice GetSlice(Vector2Int botLeft, Vector2Int dimensions)
        {
            return new GridSlice(this, botLeft, dimensions);
        }

        /// <summary>
        /// A grid slice containing entire level. This is likely very large, operating on it is inefficient, and should only be used for testing or specific thought-out cases
        /// </summary>
        public GridSlice GetGlobalSliceExpensive()
        {
            return new GridSlice(this, _topLeftCoordinate, _gridDimensions);
        }
        
        #endregion

        // Keep track of what objects are on the grid
        #region GridObjectManagement
        
        public T InstantiateGridObject<T>(T gridObjectOrPrefab, Vector2Int coordinates) where T : GridObject
        {
            T gridObject = Instantiate(gridObjectOrPrefab);
            gridObject.GridObjectInitialize(this);
            AddGridObject(gridObject, coordinates);
            return gridObject;
        }

        public void AddGridObject(GridObject gridObject, Vector2Int coordinates)
        {
            if (_gridObjects.Add(gridObject))
            {
                gridObject.GridTransform.MoveToImmediate(coordinates);
                OnAddedGridObject?.Invoke(gridObject);
            }
        }

        public void RemoveGridObject(GridObject gridObject, bool isDestroyed = false)
        {
            if (_gridObjects.Remove(gridObject))
            {
                _gridObjects.Remove(gridObject);
                OnRemovedGridObject?.Invoke(gridObject, isDestroyed);   
            }
        }

        public void DestroyGridObject(GridObject gridObject)
        {
            if (!gridObject.destroyCancellationToken.IsCancellationRequested)
            {
                Destroy(gridObject.gameObject);
                return;
            }
            
            RemoveGridObject(gridObject, true);
        }

        public bool HasGridObject(GridObject gridObject)
        {
            return _gridObjects.Contains(gridObject);
        }
        
        public IEnumerable<GridObject> GetGridObjects()
        {
            return _gridObjects;
        }
        
        #endregion

        // Build the grid
        #region GridBuilding
        
        public void OnAfterDeserialize()
        {
            BuildGrid();
        }

        private void BuildGrid()
        {
            Cells.Clear();

            HashSet<SavedGridCell> toRemove = new HashSet<SavedGridCell>();
            foreach (SavedGridCell savedGridCell in _savedGrid)
            {
                if (savedGridCell.GridCell == null)
                {
                    toRemove.Add(savedGridCell);
                }
                else
                {
                    Cells[savedGridCell.Coordinates] = savedGridCell.GridCell;   
                }
            }

            _savedGrid.RemoveAll(savedGridCell => toRemove.Contains(savedGridCell));
            
            foreach (GridCell cell in Cells.Values)
            {
                RecalculateCellNeighbours(cell);
            }
        }

        private void RecalculateCellNeighbours(GridCell cell)
        {
            if (Cells.TryGetValue(cell.Coordinates + Vector2Int.right, out GridCell right))
            {
                cell.SetRightCell(right);
            }
            
            if (Cells.TryGetValue(cell.Coordinates - Vector2Int.right, out GridCell left))
            {
                cell.SetLeftCell(left);
            }

            if (Cells.TryGetValue(cell.Coordinates + Vector2Int.up, out GridCell down))
            {
                cell.SetDownCell(down);
            }

            if (Cells.TryGetValue(cell.Coordinates - Vector2Int.up, out GridCell up))
            {
                cell.SetUpCell(up);
            }
        }
        
        public void OnBeforeSerialize()
        {
            // Empty
        }
        
        #endregion
        
        #region Enumeration
        
        public IEnumerator<GridCell> GetEnumerator()
        {
            return Cells.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        #endregion
    }
}