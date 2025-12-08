namespace ProphetAR
{
    public class AIGameTurn : GameTurn
    {
        public AIGameTurn(int turnNumber, string owner) : base(turnNumber, owner)
        {
        }

        public override void OnInitialBuild()
        {
            base.OnInitialBuild();
            
            // Execute
        }
    }
}