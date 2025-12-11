using UnityEngine;

namespace ProphetAR
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField]
        private Transform _levelParent = null;
        
        [SerializeField]
        private GameObject[] _levelPrefabs = null;
        
        public Level CurrLevel { get; private set; }
        
        public void LoadLevel(int levelIndex)
        {
            if (CurrLevel != null)
            {
                Destroy(CurrLevel.gameObject);
            }

            CurrLevel = Instantiate(_levelPrefabs[levelIndex], _levelParent).GetComponent<Level>();
        }
        
    }
}