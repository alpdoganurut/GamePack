using GamePack.Logging;

namespace GamePack.TimerSystem
{
    internal static class OperationRepeater
    {
        internal static void Repeat(OperationTreeDescription treeDescription, int? repeatCount = null)
        {
            var isCounted = repeatCount.HasValue;
            var count = 0;
            
            var controlOp = new Operation(endAction: () =>
            {
                if (isCounted) count++;
                if (isCounted && count >= repeatCount)
                    return;

                var isCancelled = treeDescription.IsCancelled();
                if (isCancelled) {
                    ManagedLog.Log($"Not restarting {nameof(OperationTreeDescription)}, root: {treeDescription.Root.Name}", ManagedLog.Type.Verbose);
                }
                else
                {
                    ManagedLog.Log($"Restarting {nameof(OperationTreeDescription)}, root: {treeDescription.Root.Name}", ManagedLog.Type.Verbose);
                    treeDescription.Start();
                }
            });
            
            treeDescription.AddOperation(controlOp);
            
            if(!treeDescription.IsWaitingOrRunning()) treeDescription.Start();
        }
        
        
    }
}