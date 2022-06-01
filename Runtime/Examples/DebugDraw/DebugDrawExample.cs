#if USING_SHAPES

// #1 Import class
using GamePack.DebugDrawSystem.DrawingMethods;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples.DebugDraw
{
    public class DebugDrawExample : MonoBehaviour
    {
        [SerializeField, Required] private Vector3[] _PolyLinePositions;
        
        private void Update()
        {
            // #2 Drawing on update
            Draw.Text(new Vector3(1.5f, 0, 0), "Update", new Color(1f, 0.35f, 0.43f));
        }

        private void Start()
        {
            // #3 Timed drawing - Text
            Draw.Text(Vector3.left, "Timed", Color.magenta, duration: 3f);
        }

        private void OnDrawGizmos()
        {
            var transform1 = transform;
            var redColor = new Color(1f, 0.35f, 0.43f);
            var halfRight = new Vector3(.5f, 0, 0);

            // #4 Various drawing methods
            Draw.Text(Vector3.right, "Test", redColor, localTransform: transform1);
            Draw.Line(halfRight,Vector3.one + halfRight, localTransform: transform1);
            Draw.Arrow(Vector3.zero, - Vector3.one, redColor, localTransform: transform1);
            Draw.Circle(Vector3.zero, .5f, Vector3.up, color: Color.red, localTransform: transform1);
            Draw.Rectangle(Vector3.zero, new Vector2(3, 4), Vector3.forward, color: Color.red, localTransform: transform1);
            Draw.Point(Vector3.zero, localTransform: transform1);
            Draw.Sphere(Vector3.zero + Vector3.up, localTransform: transform1);
            Draw.PolyLine(_PolyLinePositions);
        }
    }
}

#endif