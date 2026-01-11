using TMPro;
using UnityEngine;

namespace ProphetAR
{
    /// <summary>
    /// Paints overlays (ex: green if the cell is navigable, red for being in attack range)
    /// </summary>
    public class GridCellPainter : MonoBehaviour
    {
        [SerializeField]
        private GameObject _navigableIndicator = null;

        [SerializeField]
        private TextMeshPro _navigableNumSteps = null;
        
        public void ShowIsNavigable(bool show, int numSteps = 0)
        {
            if (show)
            {
                _navigableIndicator.gameObject.SetActive(true); 
                _navigableNumSteps.text = numSteps.ToString();
            }
            else
            {
                _navigableIndicator.gameObject.SetActive(false);   
            }
        }
    }
}