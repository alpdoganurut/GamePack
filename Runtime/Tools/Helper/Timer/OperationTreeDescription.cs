using System.Collections.Generic;

namespace GamePack.Timer
{
    public struct OperationTreeDescription
    {
        public Operation Root;
        public List<Operation> Operations;

        public void Start(bool ignoreTimeScale = false)
        {
            Engine.Instance.AddOperation(Root);
            if(ignoreTimeScale) SetIgnoreTimeScale(true);
        }

        public void Cancel()
        {
            foreach (var operation in Operations)
            {
                operation.Cancel();
            }
        }

        private void SetIgnoreTimeScale(bool isIgnore)
        {
            foreach (var operation in Operations)
            {
                operation.SetIgnoreTimeScale(isIgnore);
            }
        }
    }
}