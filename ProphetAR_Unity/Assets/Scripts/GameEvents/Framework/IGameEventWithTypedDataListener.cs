namespace ProphetAR
{
    public interface IGameEventWithTypedDataListener<TData> : IGameEventWithDataListener
    {
        public void OnEvent(TData data);
    }
}