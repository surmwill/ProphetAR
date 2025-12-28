using System.Collections.Generic;
using System.Text;

namespace ProphetAR
{
    public class LevelConfig
    {
        // Keys are the player index, values are the spawn information
        [DynamicallyConfigured]
        public Dictionary<int, List<CharacterSpawnPoint>> PlayerSpawnPoints { get; } = new();
        
        // The references (keys) aren't descriptive, but they might be useful to inspect during a breakpoint
        private readonly Dictionary<ILevelConfigContributor, string> _debugEditedBy = new();

        public void Modify(ILevelConfigContributor configContributor)
        {
            _debugEditedBy.Add(configContributor, configContributor.LevelConfigEditedBy);
            configContributor.EditLevelConfig(this);
        }

        public string DebugGetEditedBy()
        {
            StringBuilder sb = new StringBuilder($"Level config edited by ({_debugEditedBy.Count}):\n\n");
            foreach (string editedBy in _debugEditedBy.Values)
            {
                sb.AppendLine(editedBy);
            }
            return sb.ToString();
        }
    }
}