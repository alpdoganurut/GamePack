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
        private readonly List<float> _runningOperationStartTimes = new List<float>();
        private readonly List<float?> _runningOperationEndTimes = new List<float?>();
        
        public void AddOperation(Operation operation)
        {
            Log($"Adding operation {operation.Name}, delay: {operation.Delay}");
            
            _rootOperations.Add(operation);
            _rootOperationTimes.Add(Time.time + operation.Delay);
        }

        private void Update()
        {
            SyncRemove(operation => operation.State != OperationState.Waiting, _rootOperations, _rootOperationTimes);
            
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

            SyncRemove(operation => operation.State != OperationState.Running, _runningOperations, _runningOperationEndTimes, _runningOperationStartTimes);
            
            for (var index = 0; index < _runningOperations.Count; index++)
            {
                var runningOperation = _runningOperations[index];
                var endTime = _runningOperationEndTimes[index];
                var shouldUpdate = runningOperation.Duration.HasValue;

                // Resolve
                if (
                    (endTime.HasValue && Time.time > endTime) ||
                    runningOperation.IsFinished() 
                    )
                {
                    if(shouldUpdate) runningOperation.Update(1);
                    Resolve(runningOperation);
                    continue;
                }
                
                if(shouldUpdate)
                {
                    var startTime = _runningOperationStartTimes[index];
                    var duration = endTime - startTime;
                    var time = Time.time - startTime;
                    var t = time / duration;
                    runningOperation.Update(t);
                }
            }
        }

        private void RunOperation(Operation operation)
        {
            Log($"Running {operation.Name}");
            
            operation.Run();
            
            _runningOperations.Add(operation);
            _runningOperationStartTimes.Add(Time.time);
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