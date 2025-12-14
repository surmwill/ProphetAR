using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProphetAR
{
    /// <summary>
    /// A list of GridTransforms that are currently occupying the cell
    /// </summary>
    public class GridCellOccupiers : IEnumerable<GridTransform>
    {
        public event Action<GridTransform> OnOccupierAdded;
        
        public event Action<GridTransform> OnOccupierRemoved; 
        
        public IEnumerable<Character> Characters => _occupierTransforms
            .Select(occupierTransform => occupierTransform.GridObject as Character)
            .Where(character => character != null);
        
        private readonly List<GridTransform> _occupierTransforms = new();

        public void Add(GridTransform addOccupier)
        {
            if (_occupierTransforms.Contains(addOccupier))
            {
                return;
            }
            
            _occupierTransforms.Add(addOccupier);
            OnOccupierAdded?.Invoke(addOccupier);
        }

        public void Remove(GridTransform removeOccupier)
        {
            if (!_occupierTransforms.Contains(removeOccupier))
            {
                return;
            }

            _occupierTransforms.Remove(removeOccupier);
            OnOccupierRemoved?.Invoke(removeOccupier);
        }

        public IEnumerator<GridTransform> GetEnumerator()
        {
            return _occupierTransforms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}