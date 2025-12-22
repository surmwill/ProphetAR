using System;
using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    public class GameEventCharacterStoppedData
    {
        public CustomPriorityQueue<StopAction> StopActions { get; } = new();
        
        public Vector2Int StopCoordinates { get; }

        public GameEventCharacterStoppedData(Vector2Int stopCoordinates)
        {
            StopCoordinates = stopCoordinates;
        }
        
        public class StopAction : CustomPriorityQueueItem<StopAction>
        {
            public Func<IEnumerator> ExecuteStopAction { get; }
            
            public bool AfterActionCanProgress { get; }

            public StopAction(Func<IEnumerator> executeStopAction, bool afterActionCanProgress)
            {
                ExecuteStopAction = executeStopAction;
                AfterActionCanProgress = afterActionCanProgress;
            }
        }
    }
}