#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using ProphetAR.Editor;
using UnityEditor;
using UnityEngine;

namespace ProphetAR
{
    public partial class GridSection
    {
        public void SetParentGrid(CustomGrid grid)
        {
            _parentGrid = grid;
        }

        public void ClearSection()
        {
            if (_cellsParent != null)
            {
                EditorUtils.DestroyInEditMode(_cellsParent.gameObject);
            }
            _sectionDimensions = Vector2.zero;
        }
        
        public void CreateNewSection(Vector2 gridDimensions)
        {
            if (_cellsParent != null && _cellsParent.childCount > 0)
            {
                Debug.LogError($"The grid already exists, please clear before creating a new one");
                return;
            }

            if (_cellDimensions.x <= 0 || _cellDimensions.y <= 0)
            {
                Debug.LogError($"Cells cannot have zero or negative dimensions");
                return;
            }

            if (gridDimensions.x <= 0 || gridDimensions.y <= 0)
            {
                Debug.LogError("Cannot create a grid with zero or negative dimensions");
                return;
            }

            if (_cellsParent == null)
            {
                _cellsParent = new GameObject("CellsParent").GetComponent<Transform>();
                _cellsParent.SetParent(transform);
                _cellsParent.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _cellsParent.localScale = Vector3.one;   
            }

            for (int row = 0; row < gridDimensions.x; row++)
            {
                Transform rowTransform = new GameObject($"{row}").transform;
                rowTransform.SetParent(_cellsParent);
                rowTransform.SetLocalPositionAndRotation(-Vector3.forward * _cellDimensions.y * (row + 1), Quaternion.identity);
                rowTransform.localScale = Vector3.one;
                
                for (int col = 0; col < gridDimensions.y; col++)
                {
                    GridCell newCell = (GridCell) PrefabUtility.InstantiatePrefab(_cellPrefab, rowTransform);
                    newCell.SetGridSection(this);
                    newCell.SetContent(_cellContentPrefab);
                    newCell.name = CellNameForCoordinates(row, col);
                    
                    if (col > 0)
                    {
                        newCell.transform.localPosition = newCell.transform.localPosition.AddX(_cellDimensions.x * col);
                    }
                }
            }

            _sectionDimensions = gridDimensions;
        }

        public void SnapSectionsToSelf()
        {
            SetCellDimensions(_cellDimensions);
        }

        public void SetCellDimensions(Vector2 newCellDimensions)
        {
            SetCellDimensionsRecursive(newCellDimensions);
            SnapSectionsRecursive(this, new HashSet<GridSection>());
        }

        private void SetCellDimensionsRecursive(Vector2 newCellDimensions)
        {
            if (newCellDimensions.x <= 0 || newCellDimensions.y <= 0)
            {
                Debug.LogError($"Cells cannot have zero or negative dimensions");
                return;
            }
            
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                _cellDimensions = newCellDimensions;
                return;
            }
            
            // Resize this grid section
            if (_cellDimensions != newCellDimensions)
            {
                foreach (Transform rowTransform in _cellsParent)
                {
                    int row = int.Parse(rowTransform.name);
                    rowTransform.localPosition = -Vector3.forward * (newCellDimensions.y * (row + 1));
                    
                    foreach (Transform colTransform in rowTransform)
                    {
                        int col = ExtractColFromCellName(colTransform.name);
                        colTransform.localPosition = Vector3.right * (newCellDimensions.x * col);
                    }
                    
                }

                _cellDimensions = newCellDimensions;
                foreach (GridCell gridCell in GetCells())
                {
                    gridCell.EditorNotifyCellDimensionsChanged(newCellDimensions);
                }
            }

            // Resize snapped grid sections
            foreach (GridSnap gridSnap in _gridSnaps.Where(gridSnap => gridSnap.Snap != null))
            {
                gridSnap.Snap.GridSection.SetCellDimensionsRecursive(newCellDimensions);   
            } 
        }

        private void SnapSectionsRecursive(GridSection currSection, HashSet<GridSection> handledSections)
        {
            if (!handledSections.Add(currSection))
            {
                return;
            }
            
            // Snap the other grid sections to this one (this one does not move)
            _gridSnaps.ForEach(gridSnap => gridSnap.SnapTogether(this));
            
            // Then do the same for the other grid sections
            _gridSnaps.ForEach(gridSnap =>
            {
                gridSnap.Snap.GridSection.SnapSectionsRecursive(gridSnap.Snap.GridSection, handledSections);
            });
        }
        
        public void AddRightCol()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid does not exist to add to. Please create one");
                return;
            }
            
            int currNumCols = (int) _sectionDimensions.y;
            
            foreach (Transform rowTransform in _cellsParent)
            {
                GridCell newCell = (GridCell) PrefabUtility.InstantiatePrefab(_cellPrefab, rowTransform);
                newCell.SetGridSection(this);
                newCell.SetContent(_cellContentPrefab);
                newCell.name = CellNameForCoordinates(int.Parse(rowTransform.name), currNumCols);
                newCell.transform.localPosition = newCell.transform.localPosition.AddX(_cellDimensions.x * currNumCols);
            }
            
            _sectionDimensions = _sectionDimensions.AddY(1);
        }

        public void AddLeftCol()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid does not exist to add to. Please create one");
                return;
            }
            
            foreach (Transform rowTransform in _cellsParent)
            {
                int row = int.Parse(rowTransform.name);
                
                foreach (Transform cellTransform in rowTransform)
                {
                    int col = ExtractColFromCellName(cellTransform.name);
                    cellTransform.name = CellNameForCoordinates(row, col + 1);
                    cellTransform.localPosition = cellTransform.localPosition.AddX(_cellDimensions.x);
                }
                
                GridCell newCell = (GridCell) PrefabUtility.InstantiatePrefab(_cellPrefab, rowTransform);
                newCell.SetGridSection(this);
                newCell.SetContent(_cellContentPrefab);
                newCell.name = CellNameForCoordinates(row, 0);
                newCell.transform.SetAsFirstSibling();
            }

            _sectionDimensions = _sectionDimensions.AddY(1);
        }

        public void RemoveRightCol()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid does not exist to remove from. Please create one");
                return;
            }
            
            int currNumCols = (int) _sectionDimensions.y;
            
            foreach (Transform rowTransform in _cellsParent)
            {
                Transform lastCellTransform = rowTransform.GetChild(rowTransform.childCount - 1);
                int col = ExtractColFromCellName(lastCellTransform.name);
                
                if (col == currNumCols - 1)
                {
                    EditorUtils.DestroyInEditMode(lastCellTransform.gameObject);   
                }
            }

            _sectionDimensions = _sectionDimensions.AddY(-1);
        }

        public void RemoveLeftCol()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid does not exist to remove from. Please create one");
                return;
            }
            
            foreach (Transform rowTransform in _cellsParent)
            {
                foreach (Transform cellTransform in rowTransform)
                {
                    int col = ExtractColFromCellName(cellTransform.name);
                    if (col > 0)
                    {
                        cellTransform.name = CellNameForCoordinates(int.Parse(rowTransform.name), col - 1);
                        cellTransform.localPosition = cellTransform.localPosition.AddX(-_cellDimensions.x);
                    }
                    else
                    {
                        EditorUtils.DestroyInEditMode(cellTransform.gameObject);
                    }
                }
            }

            _sectionDimensions = _sectionDimensions.AddY(-1);
        }

        public void AddUpRow()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid does not exist to add to. Please create one");
                return;
            }
            
            foreach (Transform rowTransform in _cellsParent)
            {
                int row = int.Parse(rowTransform.name);
                rowTransform.name = $"{row + 1}";
                rowTransform.localPosition = rowTransform.localPosition.AddZ(-_cellDimensions.y);
            }

            Transform newRowTransform = new GameObject("0").transform;
            newRowTransform.SetParent(_cellsParent);
            newRowTransform.SetLocalPositionAndRotation(-Vector3.forward * _cellDimensions.y, Quaternion.identity);
            newRowTransform.localScale = Vector3.one;
            newRowTransform.SetAsFirstSibling();

            int currNumCols = (int) _sectionDimensions.y;
            for (int col = 0; col < currNumCols; col++)
            {
                GridCell newCell = (GridCell) PrefabUtility.InstantiatePrefab(_cellPrefab, newRowTransform);
                newCell.SetGridSection(this);
                newCell.SetContent(_cellContentPrefab);
                newCell.name = CellNameForCoordinates(0, col);

                if (col > 0)
                {
                    newCell.transform.localPosition = newCell.transform.localPosition.AddX(_cellDimensions.x * col);
                }
            }
            
            _sectionDimensions = _sectionDimensions.AddX(1);
        }

        public void AddDownRow()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid section does not exist to add to. Please create one");
                return;
            }
            
            (int currNumRows, int currNumCols) = ((int) _sectionDimensions.x, (int) _sectionDimensions.y);

            Transform newRowTransform = new GameObject($"{currNumRows}").transform;
            newRowTransform.SetParent(_cellsParent);
            newRowTransform.SetLocalPositionAndRotation(-Vector3.forward * (currNumRows + 1) * _cellDimensions.y, Quaternion.identity);
            newRowTransform.localScale = Vector3.one;
            newRowTransform.SetAsLastSibling();
            
            for (int col = 0; col < currNumCols; col++)
            {
                GridCell newCell = (GridCell) PrefabUtility.InstantiatePrefab(_cellPrefab, newRowTransform);
                newCell.SetGridSection(this);
                newCell.SetContent(_cellContentPrefab);
                newCell.name = CellNameForCoordinates(currNumRows, col);
                
                if (col > 0)
                {
                    newCell.transform.localPosition = newCell.transform.localPosition.AddX(_cellDimensions.x * col);
                }
            }

            _sectionDimensions = _sectionDimensions.AddX(1);
        }

        public void RemoveUpRow()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid does not exist to remove from. Please create one");
                return;
            }
            
            foreach (Transform rowTransform in _cellsParent)
            {
                int row = int.Parse(rowTransform.name);
                if (row > 0)
                {
                    rowTransform.localPosition = rowTransform.localPosition.AddZ(_cellDimensions.y);
                    rowTransform.name = $"{row - 1}";
                }
                else
                {
                    EditorUtils.DestroyInEditMode(rowTransform.gameObject);
                }
            }

            _sectionDimensions = _sectionDimensions.AddX(-1);
        }

        public void RemoveDownRow()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid does not exist to remove from. Please create one");
                return;
            }
            
            int currNumRows = (int) _sectionDimensions.x;
            Transform lastRow = _cellsParent.GetChild(_cellsParent.childCount - 1);
            
            if (int.Parse(lastRow.name) == currNumRows - 1)
            {
                EditorUtils.DestroyInEditMode(lastRow.gameObject);
            }

            _sectionDimensions = _sectionDimensions.AddX(-1);
        }
        
        // The cell name is expected to be the coordinates of the cell (x, y)
        private int ExtractColFromCellName(string cellName)
        {
            int indexComma = cellName.IndexOf(',');
            
            // (cellName.length - 1) - indexComma - 1
            return int.Parse(cellName.Substring(indexComma + 1, cellName.Length - indexComma - 2));
        }

        private static string CellNameForCoordinates(int row, int col)
        {
            return $"({row},{col})";
        }
        
        public IEnumerable<GridCell> GetCells()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                yield break;
            }
            
            foreach (Transform rowTransform in _cellsParent)
            {
                foreach (Transform colTransform in rowTransform)
                {
                    yield return colTransform.GetComponent<GridCell>();
                }
            }
        }

        private void OnValidate()
        {
            if (_gridSnaps == null)
            {
                return;
            }
            
            foreach (GridSnap gridSnap in _gridSnaps)
            {
                if (gridSnap.Origin != null && gridSnap.Origin.GridSection != this)
                {
                    Debug.LogWarning("The origin cell must be in this grid section");
                    gridSnap.SetSnap(null);
                }

                if (gridSnap.Snap != null && gridSnap.Snap.GridSection == this)
                {
                    Debug.LogWarning("Cannot snap a cell to its own grid");
                    gridSnap.SetOrigin(null);
                }

                if (gridSnap.Snap != null)
                {
                    gridSnap.SetSnapSection(gridSnap.Snap.GridSection);
                }
                else
                {
                    gridSnap.SetSnapSection(null);
                }
            }
        }
    }
}
#endif