using System;
using UnityEngine;

namespace ProphetAR
{
    [RequireComponent(typeof(GridTransform))]
    public abstract class GridObject : MonoBehaviour
    {
        public CustomGrid Grid { get; private set; }

        public Level Level => Grid.Level;
        
        public GridTransform GridTransform { get; private set; }
        
        public bool IsDestroying { get; private set; }

        private void Awake()
        {
            GridTransform = GetComponent<GridTransform>();
        }

        public void Initialize(CustomGrid grid)
        {
            Grid = grid;
            GridTransform.Initialize(this, grid);
        }

        private void OnDestroy()
        {
            IsDestroying = true;
            Grid.DestroyGridObject(this);
        }
    }
}