// Use #define TIMER_ENABLE_LOG to enable logs for Engine

#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GamePack.Logging;
using GamePack.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PlayerLoop;
using Debug = UnityEngine.Debug;

namespace GamePack.TimerSystem
{
    public static class TimerEngine
    {
        [ShowInInspector] private static readonly List<Operation> RootOperations = new();
        private static readonly List<float?> RootOperationTimes = new();
        
        [ShowInInspector] private static readonly List<Operation> RunningOperations = new();
        private static readonly List<float> RunningOperationStartTimes = new();
        private static readonly List<float?> RunningOperationEndTimes = new();
        
        // Optimization
        private static readonly List<Operation> OperationsToResolveImmediately = new();

        internal static void AddOperation(Operation operation)
        {
            Log($"Adding operation {operation.Name}, delay: {operation.Delay}, ignoreTimeScale: {operation.IsIgnoreTimeScale}");
            
            if (operation.Delay == 0)
            {
                RunOperation(operation);
                return;
            }
            
            RootOperations.Add(operation);
            RootOperationTimes.Add(GetTimeForOperation(operation) + operation.Delay);
        }

        #region Initialization

#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInitializeOnLoadMethod()
        {
            InitializeOnLoadMethod();
        }
#endif

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        private static void InitializeOnLoadMethod()
        {
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure);
            PlayerLoopUtilities.AppendToPlayerLoop<Update.ScriptRunBehaviourUpdate>(typeof(ManagedLog), Update);
        }

#if UNITY_EDITOR
        [InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode(EnterPlayModeOptions options)
        {
            ManagedLog.LogMethod(type: ManagedLog.Type.Structure);

            // Clear all lists
            RootOperations.Clear();
            RootOperationTimes.Clear();
            RunningOperations.Clear();
            RunningOperationStartTimes.Clear();
            RunningOperationEndTimes.Clear();
        }
#endif

        #endregion

        private static void Update()
        {
            if(!Application.isPlaying) return;
            
            // 1. Check for operation bind obj state.
            foreach (var operation in RootOperations) operation.BindObjUpdate();
            foreach (var operation in RunningOperations) operation.BindObjUpdate();
            
            #if TIMER_ENABLE_LOG
            // Logging all operations to remove
            var toRemove = RootOperations.Where(operation => operation.State != OperationState.Waiting);
            foreach (var operation in toRemove)
            {
                Log($"Will remove from {nameof(RootOperations)}. Op: {operation.Name}, state: {operation.State}");
            }
            #endif


            // 2. Remove root operations if they are no longer waiting -- Operations that are cancelled before started is removed at this stage
            SyncRemove(
                operation => operation.State != OperationState.Waiting, 
                RootOperations, 
                RootOperationTimes);

            // 3. Run Operations and resolve them if necessary
            // Operations with no duration or finish conditions are resolved after running
            // Skipped operations are resolved without running
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
                    rootOperation.IsWaitForConditionTrue()
                )
                {
                    RunOperation(rootOperation);
                    if(rootOperation.ShouldResolveImmediately()) 
                    {
                        OperationsToResolveImmediately.Add(rootOperation);
                    }
                }
            }
            // Resolve collected operations
            foreach (var operation in OperationsToResolveImmediately) Resolve(operation);

            // 4. Catch cancelled operations and run their end actions if set to true
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
            var toRemoveFromRunning = RunningOperations.Where(operation => operation.State != OperationState.Running);
            foreach (var operation in toRemoveFromRunning)
            {
                Log($"Will remove from {nameof(RunningOperations)}. Op: {operation.Name}, state: {operation.State}");
            }
            #endif
            
            // 5. Remove operations if they are no longer running
            SyncRemove(
                operation => operation.State != OperationState.Running,
                RunningOperations, 
                RunningOperationEndTimes, RunningOperationStartTimes);
            
            // 6. Update, resolve or cancel operations based on conditions
            for (var index = 0; index < RunningOperations.Count; index++)
            {
                var runningOperation = RunningOperations[index];
                var endTime = RunningOperationEndTimes[index];
                var hasUpdateTime = runningOperation.HasDuration();
                var timeForOperation = GetTimeForOperation(runningOperation);
                
                // Resolve
                if ((endTime.HasValue && timeForOperation > endTime) ||
                    runningOperation.IsFinishConditionTrue())
                {
                    // Final update call
                    if(runningOperation.Duration > 0)
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
                Log($"Operation chain ended. Last operation: {operation.Name}");
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
        
        // Removes from lookupList by condition and removes from lists by the same index
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