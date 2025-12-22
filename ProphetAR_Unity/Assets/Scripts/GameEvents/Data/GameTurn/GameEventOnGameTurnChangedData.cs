namespace ProphetAR
{
    public class GameEventOnGameTurnChangedData
    {
        public int PrevTurnNum { get; }
        
        public int NextTurnNum { get; }

        public GameEventOnGameTurnChangedData(int prevTurnNum, int nextTurnNum)
        {
            PrevTurnNum = prevTurnNum;
            NextTurnNum = nextTurnNum;
        }
    }
}