using System;

namespace ProphetAR
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HasGridPointCharAttribute : Attribute
    {
        public char GridPoint { get; }

        public HasGridPointCharAttribute(char gridPoint)
        {
            GridPoint = gridPoint;
        }
    }
}