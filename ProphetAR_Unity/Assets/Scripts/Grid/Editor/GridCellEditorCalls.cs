using UnityEngine;

namespace ProphetAR
{
    public partial class GridCell
    {
        public void SetLeftCell(GridCell left)
        {
            LeftCell = left;
        }
        
        public void SetRightCell(GridCell right)
        {
            RightCell = right;
        }
        
        public void SetUpCell(GridCell up)
        {
            UpCell = up;
        }
        
        public void SetDownCell(GridCell down)
        {
            DownCell = down;
        }

        public void SetDimensions(float newWidth, float newHeight)
        {
            if (!Mathf.Approximately(_dimensions.x, newWidth) && _coordinates.x > 0)
            {
                float widthDiff = newWidth - _dimensions.x;
                transform.localPosition = transform.localPosition.WithX(transform.position.x + widthDiff);
            }

            if (!Mathf.Approximately(_dimensions.y, newHeight) && _coordinates.y > 0)
            {
                float heightDiff = newHeight - _dimensions.y;
                transform.localPosition = transform.localPosition.WithZ((transform.position.z + heightDiff) * -1);
            }
        }
    }
}