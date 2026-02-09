using System;
using UnityEngine;

namespace ProphetAR
{
    [Serializable]
    public class CharacterConfig
    {
        [SerializeField]
        private Character _prefab = null;

        [SerializeField]
        private string _uid = null;
        
        [SerializeField]
        private CharacterStats _stats = null;

        public Character Prefab => _prefab;
        
        public string Uid => _uid;
        
        public CharacterStats Stats => _stats;
    }
}