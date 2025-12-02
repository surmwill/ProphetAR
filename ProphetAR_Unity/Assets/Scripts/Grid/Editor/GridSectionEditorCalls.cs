#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProphetAR
{
    public partial class GridSection
    {
        public void CreateNewSection(Vector2 gridDimensions, GridCellContent cellContent)
        {
            if (_cellsParent != null)
            {
                Debug.LogError($"The grid already exists, please clear it by deleting the child `{_cellsParent.name}` and retry");
                return;
            }

            if (_cellDimensions.x <= 0 || _cellDimensions.y <= 0)
            {
                Debug.LogError($"Cells cannot have zero or negative dimensions");
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

            _gridDimensions = gridDimensions;
        }

        public void SetCellDimensions(Vector2 newCellDimensions)
        {
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
        }
        
        public void AddRightCol()
        {
            int currNumCols = (int) _gridDimensions.y;
            
            foreach (Transform rowTransform in _cellsParent)
            {
                Transform newCellTransform = Instantiate(rowTransform);
                newCellTransform.name = $"{currNumCols + 1}";
                newCellTransform.localPosition = newCellTransform.localPosition.AddX(_cellDimensions.x * currNumCols);
            }
            
            _gridDimensions = _gridDimensions.AddY(1);
        }

        public void AddLeftCol()
        {
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

            _gridDimensions = _gridDimensions.AddY(1);
        }

        public void RemoveRightCol()
        {
            int currNumCols = (int) _gridDimensions.y;
            
            foreach (Transform rowTransform in _cellsParent)
            {
                Transform lastCellTransform = rowTransform.GetChild(rowTransform.childCount - 1);
                int col = int.Parse(lastCellTransform.name);

                if (col == currNumCols - 1)
                {
                    Destroy(lastCellTransform.gameObject);
                }
            }

            _gridDimensions = _gridDimensions.AddY(-1);
        }

        public void RemoveLeftCol()
        {
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
                        Destroy(cellTransform.gameObject);
                    }
                }
            }

            _gridDimensions = _gridDimensions.AddY(-1);
        }

        public void AddUpRow()
        {
            foreach (Transform rowTransform in _cellsParent)
            {
                int row = int.Parse(rowTransform.name);
                rowTransform.name = $"{row + 1}";
                rowTransform.localPosition.AddZ(-_cellDimensions.y);
            }
            
            Transform newRowTransform = Instantiate(_cellPrefab, _cellsParent).GetComponent<Transform>();
            newRowTransform.name = "0";
            newRowTransform.SetAsFirstSibling();

            int currNumCols = (int) _gridDimensions.y;
            for (int col = 0; col < currNumCols; col++)
            {
                Transform cellTransform = Instantiate(_cellPrefab, newRowTransform).GetComponent<Transform>();
                cellTransform.name = $"{col}";
            }
            
            _gridDimensions = _gridDimensions.AddX(1);
        }

        public void AddDownRow()
        {
            (int currNumRows, int currNumCols) = ((int) _gridDimensions.x, (int) _gridDimensions.y);
            
            Transform newRowTransform = Instantiate(_cellPrefab, _cellsParent).GetComponent<Transform>();
            newRowTransform.name = $"{currNumRows}";
            
            for (int col = 0; col < currNumCols; col++)
            {
                Transform cellTransform = Instantiate(_cellPrefab, newRowTransform).GetComponent<Transform>();
                cellTransform.name = $"{col}";
            }

            _gridDimensions = _gridDimensions.AddX(1);
        }

        public void RemoveUpRow()
        {
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
                    Destroy(rowTransform.gameObject);
                }
            }

            _gridDimensions = _gridDimensions.AddX(-1);
        }

        public void RemoveDownRow()
        {
            int currNumRows = (int) _gridDimensions.y;
            Transform lastRow = _cellsParent.GetChild(_cellsParent.childCount - 1);
            
            if (int.Parse(lastRow.name) == currNumRows - 1)
            {
                Destroy(lastRow.gameObject);
            }

            _gridDimensions = _gridDimensions.AddX(-1);
        }
        
        public IEnumerable<GridCell> GetCells()
        {
            if (_gridDimensions.x <= 0 || _gridDimensions.y <= 0)
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