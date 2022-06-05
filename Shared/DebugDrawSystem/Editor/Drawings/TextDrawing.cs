#if USING_SHAPES
using Shapes;
using UnityEngine;

namespace GamePack.DebugDrawSystem
{
    internal readonly struct TextDrawing : IDrawing
    {
        private readonly Vector3 _pos;
        private readonly string _text;
        
        private readonly Color _color;
        private readonly float _fontSize;
        private readonly TextAlign _textAlign;
        private readonly Transform _localTransform;
        private readonly bool _lookAtCamera;

        public TextDrawing(Vector3 pos, string text, Color? color = null,
            float fontSize = DrawInstructionDefaults.DefaultTextSize, TextAlign textAlign = TextAlign.Center,
            Transform localTransform = null, bool lookAtCamera = true)
        {
            _pos = pos;
            _text = text;
            
            _color = color ?? Color.white;
            _fontSize = fontSize;
            _textAlign = textAlign;
            
            _localTransform = localTransform;
            _lookAtCamera = lookAtCamera;
        }

        void IDrawing.Draw(Camera camera)
        {
            if(!_lookAtCamera)
            {
                Draw.Text(_pos, _text, _textAlign, _fontSize, _color);
                return;
            }
            
            // Look At Camera Logic
            var translationColumn = _localTransform
                ? _localTransform.localToWorldMatrix.GetColumn(3)
                : new Vector4(0, 0, 0, 1);
            
            var rotationMatrix = camera.transform.localToWorldMatrix;
            rotationMatrix.SetColumn(3, translationColumn);

            var pos = rotationMatrix.inverse.MultiplyPoint(_localTransform
                ? _localTransform.TransformPoint(_pos)
                : _pos);
            Draw.Matrix = rotationMatrix;

            Draw.Text(pos, _text, align: _textAlign, fontSize: _fontSize, color: _color);
            Draw.ResetMatrix();
        }

    }
}
#endif