using System;
using GamePack.Timer;
using UnityEngine;

namespace GamePack.TimerExample
{
    public class RepeatingTimerExample: MonoBehaviour
    {
        [SerializeField] private GameObject _Cube;
        [SerializeField] private float _Duration = .5f;
        [SerializeField] private Vector3 _MoveOffset = new Vector3(0, 0, 3);

        private void Start()
        {
            var startPos = _Cube.transform.position;
            var endPos = startPos + _MoveOffset;
            var op = new Operation("Repeating", duration: _Duration,
                updateAction: tVal =>
                {
                    _Cube.transform.position = Vector3.Lerp(startPos, endPos, tVal);
                }).Start();
            op.RepeatInfinite();
        }
    }
}