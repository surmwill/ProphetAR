using System;

namespace GridOperations
{
    public readonly struct ModificationStep : IEquatable<ModificationStep>
    {
        public (int row, int col) Coordinates { get; }
    
        public int Value { get; }

        public ModificationStep((int row, int col) coordinates, int value)
        {
            Coordinates = coordinates;
            Value = value;
        }

        public bool Equals(ModificationStep other)
        {
            return Coordinates.Equals(other.Coordinates) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is ModificationStep other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Coordinates, Value);
        }

        public static bool operator ==(ModificationStep left, ModificationStep right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ModificationStep left, ModificationStep right)
        {
            return !left.Equals(right);
        }
    }
}