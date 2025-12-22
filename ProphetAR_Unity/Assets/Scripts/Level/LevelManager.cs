using UnityEngine;

namespace ProphetAR
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField]
        private Transform _levelParent = null;
        
        [SerializeField]
        private Level[] _levelPrefabs = null;
        
        public Level CurrLevel { get; private set; }
        
        public void LoadLevelFromIndex(int levelIndex)
        {
            LoadLevelFromPrefab(_levelPrefabs[levelIndex]);
        }

        public void LoadLevelFromPrefab(Level levelPrefab)
        {
            if (CurrLevel != null)
            {
                Destroy(CurrLevel.gameObject);
            }

            CurrLevel = Instantiate(levelPrefab, _levelParent);
        }
    }
}