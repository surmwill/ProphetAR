using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProphetAR
{
    [Serializable]
    public class GamePlayerConfig
    {
        [SerializeField]
        private string _playerUid = null;

        [SerializeField]
        private bool _isAI = false;

        [SerializeField]
        private List<CharacterConfig> _characterConfigs = null;

        public string PlayerUid => _playerUid;

        public bool IsAI => _isAI;

        public List<CharacterConfig> CharacterConfigs => _characterConfigs;
    }
}