using System;
using UnityEngine;

namespace ProphetAR
{
    // Editor only calls. Extra functionality is needed for building the grid through the editor
    public partial class GridCell
    {
        public void SetLeftCell(GridCell cell)
        {
            LeftCell = cell;
        }
        
        public void SetRightCell(GridCell cell)
        {
            LeftCell = cell;
        }
        
        public void SetUpCell(GridCell cell)
        {
            LeftCell = cell;
        }
        
        public void SetDownCell(GridCell cell)
        {
            LeftCell = cell;
        }

        public void CopyToLeftCell()
        {
            GridCell cell = Create(transform.TransformVector(transform.localPosition.WithX(transform.localPosition.x - Dimensions.x)), _dimensions);
            _grid.AddCell(cell);
        }

        public void CopyToRightCell()
        {
            GridCell cell = Create(transform.TransformVector(transform.localPosition.WithX(transform.localPosition.x + Dimensions.x)), _dimensions);
            _grid.AddCell(cell);
        }

        public void CopyToUpCell()
        {
            GridCell cell = Create(transform.TransformVector(transform.localPosition.WithZ(transform.localPosition.z + Dimensions.y)), _dimensions);
            _grid.AddCell(cell);
        }

        public void CopyToDownCell()
        {
            GridCell cell = Create(transform.TransformVector(transform.localPosition.WithZ(transform.localPosition.z - Dimensions.y)), _dimensions);
            _grid.AddCell(cell);
        }
        
        public void AddToGrid()
        {
            _grid.AddCell(this);
        }

        public void RebuildGrid()
        {
            _grid.RebuildGrid();
        }

        public void RecalculateCoordinates()
        {
            _coordinates = new Vector2(
                Mathf.RoundToInt((transform.localPosition.x - _grid.OriginCell.transform.localPosition.x) / _grid.CellDimensions.x),
                Mathf.RoundToInt((transform.localPosition.z - _grid.OriginCell.transform.localPosition.z) * -1 / _grid.CellDimensions.y));
        }

        public void UpdateDimensions(Vector2 newDimensions)
        {
            if (!Mathf.Approximately(Dimensions.x, newDimensions.x) && _coordinates.x > _grid.MinCoordinates.x)
            {
                float widthDiff = newDimensions.x - Dimensions.x;
                transform.localPosition = transform.localPosition.WithX(transform.position.x + widthDiff);
            }

            if (!Mathf.Approximately(Dimensions.y, newDimensions.y) && _coordinates.y > _grid.MinCoordinates.y)
            {
                float heightDiff = newDimensions.y - Dimensions.y;
                transform.localPosition = transform.localPosition.WithZ((transform.position.z + heightDiff) * -1);
            }
        }
    }
}