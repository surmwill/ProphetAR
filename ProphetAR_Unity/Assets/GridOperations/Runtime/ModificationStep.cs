namespace GridOperations
{
    public struct ModificationStep
    {
        public (int row, int col) Coordinates { get; }
    
        public int Value { get; }

        public ModificationStep((int row, int col) coordinates, int value)
        {
            Coordinates = coordinates;
            Value = value;
        }
    }
}