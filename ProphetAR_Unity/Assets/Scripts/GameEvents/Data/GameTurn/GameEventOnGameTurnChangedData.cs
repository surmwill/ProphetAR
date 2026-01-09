namespace ProphetAR
{
    public class GameEventOnGameTurnChangedData
    {
        public int PrevTurnNum { get; }
        
        public int NextTurnNum { get; }
        
        public GamePlayer PrevPlayer { get; }
        
        public GamePlayer NextPlayer { get; }

        public GameEventOnGameTurnChangedData(int prevTurnNum, int nextTurnNum, GamePlayer prevPlayer, GamePlayer nextPlayer)
        {
            PrevTurnNum = prevTurnNum;
            NextTurnNum = nextTurnNum;

            PrevPlayer = prevPlayer;
            NextPlayer = nextPlayer;
        }
    }
}