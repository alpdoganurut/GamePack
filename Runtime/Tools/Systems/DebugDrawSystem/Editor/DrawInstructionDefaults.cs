using UnityEngine;

namespace GamePack.Utilities.DebugDrawSystem
{
    internal static class DrawInstructionDefaults
    {
        public const float DefaultLineThickness = 0.015f;
        public static readonly Color DefaultColor = Color.white;
        public const float DefaultPointSize = .1f;
        public const float ArrowRadiusToLength = 1.4f;
        public const float DefaultTextSize = 1f;
        public const float DefaultAxisSize = .5f;
        public const float DefaultArrowRadius = .05f;
        
        public static readonly Color AxisXColor = new Color(0.68f, 0.16f, 0.12f);
        public static readonly Color AxisYColor = new Color(0.22f, 0.67f, 0.2f);
        public static readonly Color AxisZColor = new Color(0.13f, 0.36f, 0.67f);
    }
}