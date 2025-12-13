using System;
using UnityEngine;

namespace ProphetAR
{
    [RequireComponent(typeof(GridTransform))]
    public class GridObject : MonoBehaviour
    {
        public CustomGrid Grid { get; private set; }
        
        public GridTransform GridTransform { get; private set; }

        private void Awake()
        {
            GridTransform = GetComponent<GridTransform>();
        }

        public void Initialize(CustomGrid grid)
        {
            Grid = grid;
            GridTransform.Initialize(this, grid);
        }
    }
}