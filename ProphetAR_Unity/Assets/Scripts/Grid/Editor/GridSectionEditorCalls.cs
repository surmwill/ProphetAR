#if UNITY_EDITOR
using System.Collections.Generic;
using ProphetAR.Editor;
using UnityEngine;

namespace ProphetAR
{
    public partial class GridSection
    {
        public void SetParentGrid(Grid grid)
        {
            _parentGrid = grid;
        }

        public void ClearSection()
        {
            if (_cellsParent != null)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    EditorUtils.DestroyInEditMode(_cellsParent);
                }
                else
                {
                    Destroy(_cellsParent);    
                }
            }
        }
        
        public void CreateNewSection(Vector2 gridDimensions)
        {
            if (_cellsParent != null)
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
            
            _cellsParent = new GameObject("CellsParent").GetComponent<Transform>();
            _cellsParent.SetParent(transform);
            _cellsParent.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _cellsParent.localScale = Vector3.one;

            for (int row = 0; row < gridDimensions.x; row++)
            {
                Transform rowTransform = Instantiate(_cellPrefab, _cellsParent).GetComponent<Transform>();
                rowTransform.name = $"{row}";

                if (row > 0)
                {
                    rowTransform.localPosition = rowTransform.localPosition.AddZ(-_cellDimensions.y * row);
                }
                
                for (int col = 0; col < gridDimensions.y; col++)
                {
                    Transform cellTransform = Instantiate(_cellPrefab, rowTransform).GetComponent<Transform>();
                    cellTransform.name = $"{col}";
                    
                    if (col > 0)
                    {
                        cellTransform.localPosition = cellTransform.localPosition.AddX(_cellDimensions.x * row);   
                    }
                }
            }

            _sectionDimensions = gridDimensions;
        }

        public void SetCellDimensions(Vector2 newCellDimensions)
        {
            if (newCellDimensions.x <= 0 || newCellDimensions.y <= 0)
            {
                Debug.LogError($"Cells cannot have zero or negative dimensions");
                return;
            }

            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                return;
            }
            
            float widthDiff = newCellDimensions.x - _cellDimensions.x;
            float heightDiff = newCellDimensions.y - _cellDimensions.y;
            
            foreach (Transform rowTransform in _cellsParent)
            {
                int row = int.Parse(rowTransform.name);
                if (row > 0)
                {
                    rowTransform.localPosition = rowTransform.localPosition.AddZ(-heightDiff);
                }

                foreach (Transform colTransform in rowTransform)
                {
                    int col = int.Parse(colTransform.name);
                    if (col > 0)
                    {
                        colTransform.localPosition = colTransform.localPosition.AddX(widthDiff);
                    }
                }
            }

            _cellDimensions = newCellDimensions;
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
                Transform newCellTransform = Instantiate(rowTransform);
                newCellTransform.name = $"{currNumCols + 1}";
                newCellTransform.localPosition = newCellTransform.localPosition.AddX(_cellDimensions.x * currNumCols);
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
                foreach (Transform cellTransform in rowTransform)
                {
                    int col = int.Parse(cellTransform.name);
                    cellTransform.name = $"{col + 1}";
                    cellTransform.localPosition = cellTransform.localPosition.AddX(_cellDimensions.x);
                }
                
                Transform newCellTransform = Instantiate(rowTransform);
                newCellTransform.name = "0";
                newCellTransform.SetAsFirstSibling();
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
                int col = int.Parse(lastCellTransform.name);

                if (col == currNumCols - 1)
                {
                    if (Application.isEditor && !Application.isPlaying)
                    {
                        EditorUtils.DestroyInEditMode(lastCellTransform.gameObject);   
                    }
                    else
                    {
                        Destroy(lastCellTransform.gameObject);
                    }
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
                    int col = int.Parse(cellTransform.name);
                    if (col > 0)
                    {
                        cellTransform.name = $"{col - 1}";
                        cellTransform.localPosition = cellTransform.localPosition.AddX(-_cellDimensions.x);
                    }
                    else
                    {
                        if (Application.isEditor && !Application.isPlaying)
                        {
                            EditorUtils.DestroyInEditMode(cellTransform.gameObject);
                        }
                        else
                        {
                            Destroy(cellTransform.gameObject);
                        }
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
                rowTransform.localPosition.AddZ(-_cellDimensions.y);
            }
            
            Transform newRowTransform = Instantiate(_cellPrefab, _cellsParent).GetComponent<Transform>();
            newRowTransform.name = "0";
            newRowTransform.SetAsFirstSibling();

            int currNumCols = (int) _sectionDimensions.y;
            for (int col = 0; col < currNumCols; col++)
            {
                Transform cellTransform = Instantiate(_cellPrefab, newRowTransform).GetComponent<Transform>();
                cellTransform.name = $"{col}";
            }
            
            _sectionDimensions = _sectionDimensions.AddX(1);
        }

        public void AddDownRow()
        {
            if (_sectionDimensions.x <= 0 || _sectionDimensions.y <= 0)
            {
                Debug.LogError("A grid does not exist to add to. Please create one");
                return;
            }
            
            (int currNumRows, int currNumCols) = ((int) _sectionDimensions.x, (int) _sectionDimensions.y);
            
            Transform newRowTransform = Instantiate(_cellPrefab, _cellsParent).GetComponent<Transform>();
            newRowTransform.name = $"{currNumRows}";
            
            for (int col = 0; col < currNumCols; col++)
            {
                Transform cellTransform = Instantiate(_cellPrefab, newRowTransform).GetComponent<Transform>();
                cellTransform.name = $"{col}";
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
                    if (Application.isEditor && !Application.isPlaying)
                    {
                        EditorUtils.DestroyInEditMode(rowTransform.gameObject);
                    }
                    else
                    {
                        Destroy(rowTransform.gameObject);   
                    }
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
            
            int currNumRows = (int) _sectionDimensions.y;
            Transform lastRow = _cellsParent.GetChild(_cellsParent.childCount - 1);
            
            if (int.Parse(lastRow.name) == currNumRows - 1)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    EditorUtils.DestroyInEditMode(lastRow.gameObject);
                }
                else
                {
                    Destroy(lastRow.gameObject);     
                }
            }

            _sectionDimensions = _sectionDimensions.AddX(-1);
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
    }
}
#endif