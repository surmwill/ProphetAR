using System.Collections.Generic;

namespace ProphetAR
{
    public class LevelState
    {
        public Dictionary<string, List<IGameTurnActionRequestProvider>> TurnActionRequestProvidersPerPlayer { get; } = new();

        public void AddTurnActionRequestProviderForPlayer(string playerUid, IGameTurnActionRequestProvider requestProvider)
        {
            if (!TurnActionRequestProvidersPerPlayer.TryGetValue(playerUid, out List<IGameTurnActionRequestProvider> requestProviders))
            {
                requestProviders = new List<IGameTurnActionRequestProvider>();
                TurnActionRequestProvidersPerPlayer[playerUid] = requestProviders;
            }
            requestProviders.Add(requestProvider);
        }

        public void RemoveTurnActionRequestProviderForPlayer(string playerUid, IGameTurnActionRequestProvider requestProvider)
        {
            if (!TurnActionRequestProvidersPerPlayer.TryGetValue(playerUid, out List<IGameTurnActionRequestProvider> requestProviders) || requestProviders.Count == 0)
            {
                return;
            }
            requestProviders.Remove(requestProvider);
        }
    }
}