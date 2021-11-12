// ReSharper disable InconsistentNaming

using System;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    [Serializable]
    public struct TransformInfo
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 LossyWorldScale;

        public readonly Vector3 LocalPosition;
        public readonly Quaternion LocalRotation;
        public readonly Vector3 LocalScale;
        
        public readonly Transform Parent;

        public TransformInfo(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
            LossyWorldScale = transform.lossyScale;

            LocalPosition = transform.localPosition;
            LocalRotation = transform.localRotation;
            LocalScale = transform.localScale;

            Parent = transform.parent;
        }

        public void ApplyLocal(Transform transform)
        {
            transform.SetParent(Parent, false);
            transform.localPosition = LocalPosition;
            transform.localRotation = LocalRotation;
            transform.localScale = LocalScale;
        }
        public void ApplyWorld(Transform transform)
        {
            transform.position = Position;
            transform.rotation = Rotation;

            var parent = transform.parent;
            var parentWorldScale = parent ? parent.lossyScale : Vector3.one;
            transform.localScale = new Vector3(LossyWorldScale.x / parentWorldScale.x, LossyWorldScale.y / parentWorldScale.y, LossyWorldScale.z / parentWorldScale.z);
        }
    }
}