using UnityEngine;

namespace ProphetAR
{
    public static class Vector2IntExtensions
    {
        public static Vector2Int WithX(this Vector2Int vec, int x)
        {
            vec.x = x;
            return vec;
        }
        
        public static Vector2Int WithY(this Vector2Int vec, int y)
        {
            vec.y = y;
            return vec;
        }

        public static (int, int) ToTuple(this Vector2Int vec)
        {
            return (vec.x, vec.y);
        }
    }
}