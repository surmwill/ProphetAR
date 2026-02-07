using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridOperations
{
    public static class GridRaycaster
    {
        private const float Epsilon = 1e-6f;
        
        public static bool Raycast((int row, int col) from, (int row, int col) to, IEnumerable<(int row, int col)> obstacles, out List<(int row, int col)> obstaclesHit)
        {
            obstaclesHit = new List<(int row, int col)>();
            
            if (from == to)
            {
                Debug.LogWarning("Raycasting to and from the same point");
                return false;
            }
            
            if (!obstacles?.Any() ?? true)
            {
                return false;
            }
            
            foreach ((int row, int col) obstacle in obstacles)
            {
                if (IsObstructedByObstacle(from, to, obstacle))
                {
                    obstaclesHit.Add(obstacle); 
                }
            }

            return obstaclesHit.Count > 0;
        }

        public static bool IsObstructedByObstacle((int row, int col) from, (int row, int col) to, (int row, int col) obstacle)
        {
            (float x, float y) fromMiddle = (from.col + 0.5f, from.row + 0.5f);
            (float x, float y) toMiddle = (to.col + 0.5f, to.row + 0.5f);
            
            (float x, float y) ray = (toMiddle.x - fromMiddle.x, toMiddle.y - fromMiddle.y);
            
            float tNear = float.NegativeInfinity;
            float tFar = float.PositiveInfinity;

            if (!GetDimensionalIntersection(fromMiddle.x, ray.x, obstacle.col, obstacle.col + 1, ref tNear, ref tFar))
            {
                return false;
            }

            if (!GetDimensionalIntersection(fromMiddle.y, ray.y, obstacle.row, obstacle.row + 1, ref tNear, ref tFar))
            {
                return false;
            }

            return tNear < tFar && tNear < 1f && tFar > 0f;
        }

        
        /// <summary>
        /// Returns whether there's an intersection in a given dimension
        /// </summary>
        /// <param name="start"> the starting point on the line </param>
        /// <param name="delta"> the length we can travel down the line </param>
        /// <param name="min"> the minimum point on the line for an intersection </param>
        /// <param name="max"> the maximum point on the line for an intersection </param>
        /// <param name="tNear"> the min t value in the intersection for an overlap (with an intersection in another dimension) </param>
        /// <param name="tFar"> the max t value in the intersection for an overlap (with an intersection in another dimension) </param>
        /// <returns> true if there's an intersection and overlap </returns>
        private static bool GetDimensionalIntersection(float start, float delta, int min, int max, ref float tNear, ref float tFar)
        {
            // There's nowhere to move, we're essentially given a point which is indefinitely either in or out of the intersection
            if (Math.Abs(delta) < Epsilon)
            {
                return start >= min && start <= max;
            }

            float t1 = (min - start) / delta;
            float t2 = (max - start) / delta;

            float tMin = Math.Min(t1, t2);
            float tMax = Math.Max(t1, t2);

            tNear = Math.Max(tNear, tMin);
            tFar = Math.Min(tFar, tMax);

            return true;
        }
    }
}