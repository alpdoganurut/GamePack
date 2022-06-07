using GamePack.DebugDrawSystem.DrawingMethods;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Helper.Bezier
{
    public class CurveViewer: MonoBehaviour
    {
        [SerializeField, Required] private CubicBezierCurve _Curve;

    }
}