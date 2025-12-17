using UnityEngine;

namespace ProphetAR
{
    public static class TupleExtensions
    {
        public static Vector2Int ToVector2Int(this (int, int) tuple)
        {
            return new Vector2Int(tuple.Item1, tuple.Item2);
        }
    }
}