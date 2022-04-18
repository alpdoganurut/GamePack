using GamePack.TimerSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples.TimerExample
{
    public class RepeatingTimerExample_New: MonoBehaviour
    {
        [SerializeField] private GameObject _Cube;
        [SerializeField] private float _Duration = .5f;
        [SerializeField] private Vector3 _MoveOffset = new Vector3(0, 0, 3);
        private OperationTreeDescription _repeatingDescription;

        private void Start()
        {
            var startPos = _Cube.transform.position;
            var endPos = startPos + _MoveOffset;

            _repeatingDescription = new Operation("Repeating", duration: _Duration,
                updateAction: tVal =>
                {
                    _Cube.transform.position = Vector3.Lerp(startPos, endPos, tVal);
                }).Start().Repeat();
        }

        [Button]
        private void StartMovement()
        {
            _repeatingDescription.Start();
        }

        [Button]
        private void Cancel()
        {
            _repeatingDescription.Cancel();
        }
    }
}