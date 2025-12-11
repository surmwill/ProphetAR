namespace ProphetAR
{
    public abstract class TypedGameTurnActionRequest<TGameEventWithTypedData, TGameEventData> : GameTurnActionRequest where TGameEventWithTypedData : GameEventWithTypedData<TGameEventData>
    {
        public override bool IsCompletedByGameEvent(GameEvent gameEvent)
        {
            if (gameEvent is TGameEventWithTypedData gameEventWithTypedData)
            {
                return IsCompletedByGameEventWithTypedData(gameEventWithTypedData);
            }

            return false;
        }

        protected abstract bool IsCompletedByGameEventWithTypedData(GameEventWithTypedData<TGameEventData> gameEventWithTypedData);
    }
}