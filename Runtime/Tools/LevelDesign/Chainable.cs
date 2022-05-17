#if USING_SHAPES
using GamePack.Utilities.DebugDrawSystem.DrawingMethods;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GamePack.Logging;
using GamePack.TimerSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack.LevelDesign
{
    [ExecuteAlways]
    public class Chainable: MonoBehaviour
    {
        private const float TRANSFORM_CHILDREN_CHANGED_REFRESH_DELAY = .05f;

        [SerializeField, Required, HideIf("IsParent")] private Transform _Start;
        [SerializeField, Required, HideIf("IsParent")] private Transform _End;

        private Transform Start => IsParent ? _Children[0].Start : _Start;

        private Transform End => IsParent ? _Children[^1].End : _End;
        
        public bool IsParent => _Children is {Count: > 0};

        [FormerlySerializedAs("_Modules")] [SerializeField, Required/*, OnValueChanged("PlaceModules")*/]
        private List<Chainable> _Children;


        [Button(ButtonSizes.Large)]
        private void PlaceModules()
        {
            if (!IsParent) return;

#if UNITY_EDITOR && USING_SHAPES
            RefreshChildren();
#endif

            // Align first child
            var (firstChildPos, firstChildRot) = GetAlignedPosAndRotForParent(transform, _Children[0].Start);
            _Children[0].transform.SetPositionAndRotation(firstChildPos, firstChildRot);
            
            foreach (var chainableGroup in _Children)
                chainableGroup.PlaceModules();
                
            for (var index = 0; index < _Children.Count - 1; index++)
            {
                var module = _Children[index];
                var nextModule = _Children[index + 1];
                ConnectTwoModules(module, nextModule);
            }
        }

        private static void ConnectTwoModules(Chainable module, Chainable nextModule)
        {
            var (pos, rot) = GetAlignedPosAndRotForParent(module.End, nextModule.Start);
            nextModule.transform.SetPositionAndRotation(pos, rot);
        }

        private static (Vector3 pos, Quaternion rot) GetAlignedPosAndRotForParent(Transform targetTransform, Transform childTransformToAlign)
        {
            return (
                pos: targetTransform.position -
                     childTransformToAlign.transform.TransformDirection(childTransformToAlign.localPosition),
                rot: Quaternion.LookRotation(targetTransform.forward) * Quaternion.Inverse(childTransformToAlign.localRotation)
                );
        }

#if UNITY_EDITOR && USING_SHAPES

        private void OnTransformChildrenChanged() => Invoke(nameof(PlaceModulesRecursiveUpwards), TRANSFORM_CHILDREN_CHANGED_REFRESH_DELAY);

        private void PlaceModulesRecursiveUpwards() // TODO: Method name is misleading, this call is not recursive (anymore)
        {
            var parentChainable = GetComponentsInParent<Chainable>().FirstOrDefault(chainable => chainable != this);
            if (parentChainable) parentChainable.PlaceModules();
            else PlaceModules();
        }

        [Button]
        private void RefreshChildren()
        {
            _Children.Clear();
            foreach (Transform child in transform)
            {
                var chainableChild = child.GetComponent<Chainable>();
                if(chainableChild) _Children.Add(chainableChild);
            }
        }   

        private void OnDrawGizmos()
        {
            if(IsParent) return;
            if(!IsParent && (!_Start || !_End)) return;
            
            var isSelected = UnityEditor.Selection.activeGameObject &&
                             (UnityEditor.Selection.activeGameObject == gameObject ||
                              UnityEditor.Selection.activeGameObject.transform.IsChildOf(transform));
            // Gizmos.color = isSelected ? Color.blue : Color.yellow;
            // Gizmos.DrawLine(Start.position, End.position);
            Draw.Arrow(Start.position, End.position, isSelected ? Color.blue : Color.yellow);
            Draw.Axis(Vector3.zero, Start.transform, .1f);
            Draw.Axis(Vector3.zero, End.transform, .1f);
        }

        [Button]
        private void AlignStartPoint()
        {
            // Align with start
            var transformLocalPosition = Start.transform.localPosition;
            if (transformLocalPosition.sqrMagnitude > 0)
            {
                foreach (Transform child in transform)
                {
                    child.transform.localPosition -= transformLocalPosition;
                }

                transform.position += transform.TransformDirection(transformLocalPosition);
            }
        }

        private void OnValidate()
        {
            if (Start) Start.name = "[Start]";
            if (End) End.name = "[End]";
        }
#endif
    }
}