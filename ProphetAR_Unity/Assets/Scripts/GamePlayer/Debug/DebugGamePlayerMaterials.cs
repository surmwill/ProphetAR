using UnityEngine;

namespace ProphetAR
{
    [CreateAssetMenu(menuName = "ProphetAR/GamePlayer/Debug/PlayerMaterials")]
    public class DebugGamePlayerMaterials : ScriptableObject
    {
        [SerializeField]
        private Material[] _playerMaterials = null;

        public Material[] PlayerMaterials => _playerMaterials;
    }
}
