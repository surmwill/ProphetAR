namespace GridPathFinding
{
    public struct NavigationInstruction
    {
        public enum NavigationDirection
        {
            Left = 0,
            Right = 1,
            Up = 2,
            Down = 3,
        }   
        
        public NavigationDirection Direction { get; }
        
        public int Magnitude { get; private set; }

        public void IncrementMagnitude(int increment = 1)
        {
            Magnitude += increment;
        }

        public NavigationInstruction(NavigationDirection direction, int magnitude = 1)
        {
            Direction = direction;
            Magnitude = magnitude;
        }
    }
}