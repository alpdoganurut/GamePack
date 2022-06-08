using GamePack.TimerSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Helper.Bezier
{
    public class CurveMovementTest: MonoBehaviour
    {
        [SerializeField, Required] private CubicBezierCurve _curve;
        
        [Button]
        private void Move()
        {
            new Operation(duration: 5, updateAction: val =>
            {
                transform.position = _curve.GetPointByRatio(val);
            }).Start();
        }
    }
}