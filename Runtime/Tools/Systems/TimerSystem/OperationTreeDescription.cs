using System.Collections.Generic;
using UnityEngine.Assertions;

namespace GamePack.Timer
{
    public struct OperationTreeDescription
    {
        public Operation Root;
        public List<Operation> Operations;
        public List<Operation> Tips;

        public void Start(bool ignoreTimeScale = false)
        {
            foreach (var operation in Operations)
            {
                operation.SetWaiting();
            }
            
            TimerEngine.AddOperation(Root);
            if(ignoreTimeScale) SetIgnoreTimeScale(true);
        }

        public void Cancel()
        {
            foreach (var operation in Operations)
            {
                operation.Cancel();
            }
        }

        public void RepeatInfinite()
        {
            Assert.IsTrue(Tips.Count == 1, "Can't repeat operation tree with branches. (Has more than 1 tip)");
            
            var tip = Tips[0];

            var thisDescription = this;
            var controlOperation = new Operation("Repeat Control Operation", endAction: () =>
            {
                thisDescription.Start();
            });
            
            controlOperation.SetWaiting();
            
            thisDescription.Operations.Add(controlOperation);
            
            tip.Add(controlOperation);
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