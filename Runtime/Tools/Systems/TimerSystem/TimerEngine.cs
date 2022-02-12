// Use #define TIMER_ENABLE_LOG to enable logs for Engine

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GamePack.Logging;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PlayerLoop;

namespace GamePack.Timer
{
    public static class TimerEngine
    {
        // private static Engine _instance;
        /*public static Engine Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = new GameObject("TimerEngine").AddComponent<Engine>();
                }

                return _instance;
            }
        }*/
        
        [ShowInInspector] private static readonly List<Operation> RootOperations = new List<Operation>();
        private static readonly List<float?> RootOperationTimes = new List<float?>();
        
        [ShowInInspector] private static readonly List<Operation> RunningOperations = new List<Operation>();
        private static readonly List<float> RunningOperationStartTimes = new List<float>();
        private static readonly List<float?> RunningOperationEndTimes = new List<float?>();
        
        // Optimization
        private static readonly List<Operation> OperationsToResolveImmediately = new List<Operation>();

        public static void AddOperation(Operation operation)
        {
            Log($"Adding operation {operation.Name}, delay: {operation.Delay}, ignoreTimeScale: {operation.IsIgnoreTimeScale}");
            
            RootOperations.Add(operation);
            RootOperationTimes.Add(GetTimeForOperation(operation) + operation.Delay);
        }
        
#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoadMethod()
        {
            InitializeOnLoadMethod();
        }
#endif

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.Log($"{nameof(TimerEngine)}.{nameof(InitializeOnLoadMethod)}", ManagedLog.Type.Structure);
            PlayerLoopUtilities.AppendToPlayerLoop<Update.ScriptRunBehaviourUpdate>(typeof(ManagedLog), Update);
            
        }
    
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            ManagedLog.Log($"{nameof(TimerEngine)}.{nameof(InitializeOnEnterPlayMode)}", ManagedLog.Type.Structure);

            // Clear all lists
            RootOperations.Clear();
            RootOperationTimes.Clear();
            RunningOperations.Clear();
            RunningOperationStartTimes.Clear();
            RunningOperationEndTimes.Clear();
        }

        private static void Update()
        {
            if(!Application.isPlaying) return;

            #if TIMER_ENABLE_LOG
            // Logging all operations to remove
            var toRemove = RootOperations.Where(operation => operation.State != OperationState.Waiting);
            foreach (var operation in toRemove)
            {
                Log($"Will remove from {nameof(RootOperations)}. Op: {operation.Name}, state: {operation.State}");
            }
            #endif
            
            // Remove root operations if they are no longer waiting -- Operations that are cancelled before started is removed at this stage
            SyncRemove(operation => operation.State != OperationState.Waiting, RootOperations, RootOperationTimes);

            // Run Operations
            OperationsToResolveImmediately.Clear();
            for (var index = 0; index < RootOperations.Count; index++)
            {
                var rootOperation = RootOperations[index];
                var rootOperationTime = RootOperationTimes[index];
                var timeForOperation = GetTimeForOperation(rootOperation);

                if (rootOperation.ShouldSkip())
                {
                    OperationsToResolveImmediately.Add(rootOperation);
                    continue;
                }

                if (timeForOperation > rootOperationTime &&
                    rootOperation.WaitForCondition()
                )
                {
                    RunOperation(rootOperation);
                    if(rootOperation.ShouldResolveImmediately())
                    {
                        OperationsToResolveImmediately.Add(rootOperation);
                    }
                }
            }
            // Delete 
            foreach (var operation in OperationsToResolveImmediately) Resolve(operation);

            // Catch cancelled operations and run their end actions if set to true
            foreach (var runningOperation in RunningOperations)
            {
                if (runningOperation.State == OperationState.Cancelled &&
                    runningOperation.IsRunEndActionBeforeCancel)
                {
                    runningOperation.Finish();
                }
            }
            
            #if TIMER_ENABLE_LOG
            // Logging all operations to remove
            var toRemoveFromRunning = RootOperations.Where(operation => operation.State != OperationState.Running);
            foreach (var operation in toRemoveFromRunning)
            {
                Debug.Log($"Will remove from {nameof(RunningOperations)}. Op: {operation.Name}, state: {operation.State}");
            }
            #endif
            // Remove operations if they are no longer running
            SyncRemove(operation => operation.State != OperationState.Running, RunningOperations, RunningOperationEndTimes, RunningOperationStartTimes);
            
            // Update, resolve or cancel operations based on conditions
            for (var index = 0; index < RunningOperations.Count; index++)
            {
                var runningOperation = RunningOperations[index];
                var endTime = RunningOperationEndTimes[index];
                var hasUpdateTime = runningOperation.Duration != null;
                var timeForOperation = GetTimeForOperation(runningOperation);
                
                // Resolve
                if ((endTime.HasValue && timeForOperation > endTime) ||
                    runningOperation.IsFinisCondition())
                {
                    if(runningOperation.Duration.HasValue)
                        runningOperation.Update( 1);
                    
                    Resolve(runningOperation);
                }
                else if(hasUpdateTime)
                {
                    var startTime = RunningOperationStartTimes[index];
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

        private static void RunOperation(Operation operation)
        {
            var timeForOperation = GetTimeForOperation(operation);
            Log($"Running {operation.Name}");
            
            operation.Run();
            
            RunningOperations.Add(operation);
            RunningOperationStartTimes.Add(timeForOperation);
            RunningOperationEndTimes.Add(timeForOperation + operation.Duration);
        }

        private static void Resolve(Operation operation)
        {
            Log($"Resolving {operation.Name}");
            
            SyncRemove(op => op == operation, RootOperations, RootOperationTimes);
            
            operation.Finish();
            
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
        private static void Log(object obj)
        {
            ManagedLog.Log(obj + $"\t Time: {Time.time}");
        }

        private static float GetTimeForOperation(Operation operation)
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

            removalIndexArray.Sort((i1, i2) => i2 - i1);
            
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