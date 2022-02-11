/*using UnityEngine.Assertions;

namespace GamePack.Timer
{
    public class Repeater
    {
        private readonly OperationTreeDescription _operationDescription;

        public Repeater(OperationTreeDescription operationDescription)
        {
            _operationDescription = operationDescription;

            Assert.IsTrue(_operationDescription.Tips.Count == 1, "Can't repeat operation tree with branches. (Has more than 1 tip)");

            var tip = _operationDescription.Tips[0];
            tip.Add(_operationDescription.Root);
        }
    }
}*/