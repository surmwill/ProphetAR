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

        private void Awake()
        {
            GridTransform = GetComponent<GridTransform>();
        }

        public void GridObjectInitialize(CustomGrid grid)
        {
            Grid = grid;
            GridTransform.Initialize(this, grid);
        }

        protected virtual void OnDestroy()
        {
            Grid.DestroyGridObject(this);
        }
    }
}