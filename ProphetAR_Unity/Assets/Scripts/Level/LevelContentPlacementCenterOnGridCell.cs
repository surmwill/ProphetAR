using UnityEngine;

namespace ProphetAR
{
    public class LevelContentPlacementCenterOnGridCell : MonoBehaviour, IARContentPlacementPositioner
    {
        [SerializeField]
        private Level _level = null;
        
        [SerializeField]
        private GridCell _gridCell = null;

        public Vector3 GetPosition()
        {
            return _gridCell.Middle;
        }
    }
}