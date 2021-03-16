using System;
using UnityEngine;

namespace GamePack.Animation
{
    public class AnchorToPoint: MonoBehaviour
    {
        [SerializeField] private Transform _Stabilizer;
        [SerializeField] private Transform _Anchor;
        private Vector3 _anchorPos;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 0f, 1f, 0.47f);
            Gizmos.DrawSphere(_anchorPos, .3f);
        }

        private void Awake()
        {
            _anchorPos = _Anchor.position;
        }

        private void LateUpdate()
        {
            var dif = _anchorPos - _Anchor.position;
            _Stabilizer.transform.position += dif;
        }
    }
}