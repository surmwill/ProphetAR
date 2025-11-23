namespace GridPathFinding
{
    public struct ModificationStep
    {
        public (int row, int col) Position { get; }
    
        public int Value { get; }

        public ModificationStep((int row, int col) position, int value)
        {
            Position = position;
            Value = value;
        }
    }
}