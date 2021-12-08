using System.Collections.Generic;

namespace GamePack.Timer
{
    public struct OperationTreeDescription
    {
        public Operation Root;
        public List<Operation> Operations;

        public void Start()
        {
            Engine.Instance.AddOperation(Root);
        }

        public void Cancel()
        {
            foreach (var operation in Operations)
            {
                operation.Cancel();
            }
        }
    }
}