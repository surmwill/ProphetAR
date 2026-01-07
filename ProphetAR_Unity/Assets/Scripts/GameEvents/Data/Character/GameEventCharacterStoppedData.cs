using System;
using System.Collections;
using UnityEngine;

namespace ProphetAR
{
    public class GameEventCharacterStoppedData
    {
        public CustomPriorityQueue<OnStopAction> StopActions { get; } = new();
        
        public Vector2Int StopCoordinates { get; }

        public GameEventCharacterStoppedData(Vector2Int stopCoordinates)
        {
            StopCoordinates = stopCoordinates;
        }
        
        public class OnStopAction : CustomPriorityQueueItem<OnStopAction>
        {
            public Func<IEnumerator> StopCoroutine { get; }
            
            public Action StopAction { get; }

            public StopActionExecutionType ExecutionMethod
            {
                get
                {
                    if (StopCoroutine == null && StopAction == null)
                    {
                        Debug.LogWarning("Expected to perform a coroutine or action at stop, but was given neither");
                    }

                    return StopCoroutine != null ? StopActionExecutionType.Coroutine : StopActionExecutionType.Action;
                }
            }
            
            public bool CanProgressAfterStop { get; }

            public OnStopAction(Func<IEnumerator> stopCoroutine, bool canProgressAfterStop = true)
            {
                StopCoroutine = stopCoroutine;
                CanProgressAfterStop = canProgressAfterStop;
            }
            
            public OnStopAction(Action stopAction, bool canProgressAfterStop = true)
            {
                StopAction = stopAction;
                CanProgressAfterStop = canProgressAfterStop;
            }

            public enum StopActionExecutionType
            {
                Coroutine = 0,
                Action = 1,
            }
        }
    }
}