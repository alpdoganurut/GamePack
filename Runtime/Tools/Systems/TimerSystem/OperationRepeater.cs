using GamePack.Logging;

namespace GamePack.TimerSystem
{
    internal static class OperationRepeater
    {
        internal static void Repeat(OperationTreeDescription treeDescription)
        {
            var controlOp = new Operation(endAction: () =>
            {
                var isCancelled = treeDescription.IsCancelled();
                if (isCancelled) {
                    ManagedLog.Log($"Not restarting {nameof(OperationTreeDescription)}, root: {treeDescription.Root.Name}");
                }
                else
                {
                    ManagedLog.Log($"Restarting {nameof(OperationTreeDescription)}, root: {treeDescription.Root.Name}");
                    treeDescription.Start();
                }
            });
            treeDescription.AddOperation(controlOp);
            
            if(!treeDescription.IsStarted())
            {
                treeDescription.Start();
            }
        }
        
        
    }
}