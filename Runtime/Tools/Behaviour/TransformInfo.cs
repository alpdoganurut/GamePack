// ReSharper disable InconsistentNaming

using System;
using UnityEngine;

namespace BabyCatcher.GamePack
{
    [Serializable]
    public struct TransformInfo
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 LossyScale;

        public readonly Vector3 LocalPosition;
        public readonly Quaternion LocalRotation;
        public readonly Vector3 LocalScale;
        
        public readonly Transform Parent;

        public TransformInfo(Transform transform)
        {
            Position = transform.position;
            Rotation = transform.rotation;
            LossyScale = transform.lossyScale;

            LocalPosition = transform.localPosition;
            LocalRotation = transform.localRotation;
            LocalScale = transform.localScale;

            Parent = transform.parent;
        }

        public void SetLocal(Transform transform)
        {
            transform.SetParent(Parent, false);
            transform.localPosition = LocalPosition;
            transform.localRotation = LocalRotation;
            transform.localScale = LocalScale;
        }
    }
}