#if UNITY_EDITOR
using UnityEngine;

namespace ProphetAR
{
    public partial class Grid
    {
        private const string CellsParent = "Cells";
        
        public void ParseGridFromOrigin()
        {
            // BFS and mark leftmost, rightmost, topmost, botmost
        }

        public void AddOrigin()
        {
            if (_origin != null)
            {
                return;
            }
            
            AddCellsParent();
            
        }
        
        private void OnValidate()
        {
            AddCellsParent();
        }

        private void AddCellsParent()
        {
            if (_cellsParent != null)
            {
                return;
            }
            
            _cellsParent = new GameObject { name = CellsParent }.transform;
            _cellsParent.SetParent(gameObject.transform);
            _cellsParent.SetLocalPositionAndRotation(Vector3.one, Quaternion.identity);
            _cellsParent.localScale = Vector3.one;
        }
    }
}

#endif