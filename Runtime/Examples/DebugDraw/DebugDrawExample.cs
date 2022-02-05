using GamePack.Utilities.DebugDrawSystem.DrawingMethods;
using UnityEngine;

namespace Examples.DebugDraw
{
    public class DebugDrawExample : MonoBehaviour
    {
        private void Update() => Draw.Text(new Vector3(1.5f, 0, 0), "Update", new Color(1f, 0.35f, 0.43f));

        private void Start() => Draw.Text(Vector3.left, "Timed", Color.magenta, duration: 3f);

        private void OnDrawGizmos()
        {
            var position = transform.position;
            var redColor = new Color(1f, 0.35f, 0.43f);
            var halfRight = new Vector3(.5f, 0, 0);

            Draw.Text(Vector3.right, "Test", redColor);
            Draw.Line(position + halfRight, position + Vector3.one + halfRight);
            Draw.Arrow(position, position - Vector3.one, redColor);
            Draw.Circle(position, .5f, Vector3.up, color: Color.red);
            Draw.Point(position);
            Draw.Sphere(position + Vector3.up);
        }
    }
}