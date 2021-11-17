// Use #define TIMER_ENABLE_LOG to enable logs for Engine

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GamePack.Timer
{
    public class Engine: MonoBehaviour
    {
        private static Engine _instance;
        public static Engine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("TimerEngine").AddComponent<Engine>();
                }

                return _instance;
            }
        }
        
        private readonly List<Operation> _rootOperations = new List<Operation>();
        private readonly List<float?> _rootOperationTimes = new List<float?>();
        
        private readonly List<Operation> _runningOperations = new List<Operation>();
        private readonly List<float?> _runningOperationEndTimes = new List<float?>();
        
        public void AddOperation(Operation operation)
        {
            Log($"Adding operation {operation.Name}");
            
            _rootOperations.Add(operation);
            _rootOperationTimes.Add(Time.time + operation.Delay);
        }

        private void Update()
        {
            SyncRemove(_rootOperations, _rootOperationTimes, operation => operation.State != OperationState.Waiting);
            
            for (var index = 0; index < _rootOperations.Count; index++)
            {
                var rootOperation = _rootOperations[index];
                var rootOperationTime = _rootOperationTimes[index];

                if (rootOperation.ShouldSkip())
                {
                    Resolve(rootOperation);
                    continue;
                }
                
                if (Time.time > rootOperationTime &&
                    rootOperation.WaitForCondition()
                )
                {
                    RunOperation(rootOperation);
                    if(rootOperation.ShouldResolveImmediately()) Resolve(rootOperation);
                }
            }

            SyncRemove(_runningOperations, _runningOperationEndTimes, operation => operation.State != OperationState.Running);
            
            for (var index = 0; index < _runningOperations.Count; index++)
            {
                var runningOperation = _runningOperations[index];
                var runningOperationEndTime = _runningOperationEndTimes[index];

                if (
                    (runningOperationEndTime.HasValue && Time.time > runningOperationEndTime) ||
                    runningOperation.IsFinished() 
                    )
                {
                    Resolve(runningOperation);
                    return;
                }

                runningOperation.Update();
            }
        }

        private void RunOperation(Operation operation)
        {
            Log($"Running {operation.Name}");
            
            operation.Run();
            
            _runningOperations.Add(operation);
            _runningOperationEndTimes.Add(Time.time + operation.Duration);
        }

        private void Resolve(Operation operation)
        {
            Log($"Resolving {operation.Name}");
            
            operation.SetFinished();
            
            if (operation.Children.Count > 0)
            {
                foreach (var childOperation in operation.Children)
                {
                    AddOperation(childOperation);
                }
            }
        }

        [Conditional("TIMER_ENABLE_LOG")]
        private void Log(object obj)
        {
            Debug.Log(obj + $"\t {Time.time}");
        }

        // Removes from list1 by condition and removes from list2 by the same index
        private static void SyncRemove<T1, T2>(IList<T1> list1, IList<T2> list2, Func<T1, bool> condition)
        {
            var removalArray = new List<int>();
            for (var index = 0; index < list1.Count; index++)
            {
                var item1 = list1[index];

                if (condition.Invoke(item1))
                {
                    removalArray.Add(index);
                }
            }

            foreach (var i in removalArray)
            {
                list1.RemoveAt(i);
                list2.RemoveAt(i);
            }
        }
    }
}