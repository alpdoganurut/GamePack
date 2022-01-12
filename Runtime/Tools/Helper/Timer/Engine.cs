// Use #define TIMER_ENABLE_LOG to enable logs for Engine

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Assertions;
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
                if (!_instance)
                {
                    _instance = new GameObject("TimerEngine").AddComponent<Engine>();
                }

                return _instance;
            }
        }
        
        private readonly List<Operation> _rootOperations = new List<Operation>();
        private readonly List<float?> _rootOperationTimes = new List<float?>();
        
        private readonly List<Operation> _runningOperations = new List<Operation>();
        private readonly List<float> _runningOperationStartTimes = new List<float>();
        private readonly List<float?> _runningOperationEndTimes = new List<float?>();
        
        public void AddOperation(Operation operation)
        {
            Log($"Adding operation {operation.Name}, delay: {operation.Delay}, ignoreTimeScale: {operation.IsIgnoreTimeScale}");
            
            _rootOperations.Add(operation);
            _rootOperationTimes.Add(GetTimeForOperation(operation) + operation.Delay);
        }

        private void Update()
        {
            // Remove root operations if they are no longer waiting -- Operations that are cancelled before started is removed at this stage
            SyncRemove(operation => operation.State != OperationState.Waiting, _rootOperations, _rootOperationTimes);
            
            // Start operations if ready
            for (var index = 0; index < _rootOperations.Count; index++)
            {
                var rootOperation = _rootOperations[index];
                var rootOperationTime = _rootOperationTimes[index];
                var timeForOperation = GetTimeForOperation(rootOperation);

                if (rootOperation.ShouldSkip())
                {
                    Resolve(rootOperation);
                    continue;
                }

                if (timeForOperation > rootOperationTime &&
                    rootOperation.WaitForCondition()
                )
                {
                    RunOperation(rootOperation);
                    if(rootOperation.ShouldResolveImmediately()) Resolve(rootOperation);
                }
            }

            // Catch cancelled operations and run their end actions if set to true
            foreach (var runningOperation in _runningOperations)
            {
                if (runningOperation.State == OperationState.Cancelled &&
                    runningOperation.IsRunEndActionBeforeCancel)
                {
                    runningOperation.SetFinished();
                }
            }
            
            // Remove operations if they are no longer running
            SyncRemove(operation => operation.State != OperationState.Running, _runningOperations, _runningOperationEndTimes, _runningOperationStartTimes);
            
            // Update, resolve or cancel operations based on conditions
            for (var index = 0; index < _runningOperations.Count; index++)
            {
                var runningOperation = _runningOperations[index];
                var endTime = _runningOperationEndTimes[index];
                var hasUpdateTime = runningOperation.Duration != null;
                var timeForOperation = GetTimeForOperation(runningOperation);
                
                // Resolve
                if (endTime.HasValue && timeForOperation > endTime ||
                    runningOperation.IsFinished())
                {
                    if(runningOperation.Duration.HasValue)
                        runningOperation.Update( 1);
                    
                    Resolve(runningOperation);
                }
                else if(hasUpdateTime)
                {
                    var startTime = _runningOperationStartTimes[index];
                    var duration = endTime - startTime;
                    var time = timeForOperation - startTime;
                    var t = time / duration;

                    // TODO: This is hacky - this happens if for some reason (MoveOperation, 0 distance and speed supplied) duration is 0 
                    if (t != null && float.IsNaN(t.Value))
                    {
                        t = 1;
                    }
                    
                    runningOperation.Update(t);
                }
                else
                    runningOperation.Update(null);
            }
        }

        private void RunOperation(Operation operation)
        {
            var timeForOperation = GetTimeForOperation(operation);
            Log($"Running {operation.Name}");
            
            operation.Run();
            
            _runningOperations.Add(operation);
            _runningOperationStartTimes.Add(timeForOperation);
            _runningOperationEndTimes.Add(timeForOperation + operation.Duration);
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
            else
            {
                Log("Operation chain ended");
            }
        }

        [Conditional("TIMER_ENABLE_LOG")]
        private void Log(object obj)
        {
            Debug.Log(obj + $"\t Time: {Time.time}");
        }

        private float GetTimeForOperation(Operation operation)
        {
            return operation.IsIgnoreTimeScale ? Time.unscaledTime : Time.time;
        }
        
        // Removes from list1 by condition and removes from list2 by the same index
        private static void SyncRemove<T1>(Func<T1, bool> firstListRemovalCondition, IList<T1> lookupList, params IList[] lists)
        {
            Assert.IsTrue(lists.Length > 0);
            // Assert that all lists have same number of items 
            for (var index = 0; index < lists.Length - 1; index++)
            {
                var list = lists[index];
                var nextList = lists[index];
                Assert.IsTrue(list.Count == nextList.Count);
            }

            // Find removal indices
            var removalIndexArray = new List<int>();
            for (var index = 0; index < lookupList.Count; index++)
            {
                var item1 = lookupList[index];

                if (firstListRemovalCondition.Invoke(item1))
                {
                    removalIndexArray.Add(index);
                }
            }

            removalIndexArray.Sort((int i1, int i2) => i2 - i1);
            
            // Remove from all arrays
            foreach (var i in removalIndexArray)
            {
                lookupList.RemoveAt(i);
                foreach (var list in lists)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}