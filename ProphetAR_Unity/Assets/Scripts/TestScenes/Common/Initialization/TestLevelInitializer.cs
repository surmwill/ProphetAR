using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    public class TestLevelInitializer : MonoBehaviour
    {
        [SerializeField]
        private Level _level = null;

        [SerializeField]
        private GamePlayerConfig[] _playerConfigs = null;

        private void OnEnable()
        {
            StartCoroutine(InitializeLevel());
        }

        private IEnumerator InitializeLevel()
        {
            yield return _level.InitializeCoroutine(_playerConfigs);
            _level.StartFirstTurn();
        }
    }
}