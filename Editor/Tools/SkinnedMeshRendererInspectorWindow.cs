using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GamePack.Editor.Tools
{
    [TypeInfoBox("@_skinnedMeshRenderer ? _skinnedMeshRenderer.name : \"Select a SkinnedMeshRenderer to inspect bones.\"")]
    public class SkinnedMeshRendererInspectorWindow : OdinEditorWindow
    {
        
        private SkinnedMeshRenderer _skinnedMeshRenderer;

        [ShowInInspector, ShowIf("@_skinnedMeshRenderer")] private Transform[] Bones => _skinnedMeshRenderer ? _skinnedMeshRenderer.bones : null;
        
        [MenuItem("Utilities/Skinned Mesh Renderer Inspector")]
        public static void ShowWindow()
        {
            GetWindow<SkinnedMeshRendererInspectorWindow>();
        }

        private void Awake()
        {
            Selection.selectionChanged += () =>
            {
                if(!Selection.activeGameObject) return;
                
                var smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr) _skinnedMeshRenderer = smr;
            };
        }

        [Button, InfoBox("Copy bone information from a SkinnedMeshRenderer. Bones must have matching names.")]
        private void CopyBonesOfAnother(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            const string errorMessage = "Bones are not matching!";
            
            if (skinnedMeshRenderer.bones.Length != _skinnedMeshRenderer.bones.Length)
            {
                Debug.Log(errorMessage);
                return;
            }
            var newBoneArray = new Transform[_skinnedMeshRenderer.bones.Length];
            for (var index = 0; index < _skinnedMeshRenderer.bones.Length; index++)
            {
                var ourBone = _skinnedMeshRenderer.bones[index];
                var otherBone = skinnedMeshRenderer.bones.FirstOrDefault(b => b.name == ourBone.name);
                if (otherBone)
                {
                    newBoneArray[index] = otherBone;
                }
                else
                {
                    Debug.Log(errorMessage);
                    return;
                }
            }

            _skinnedMeshRenderer.bones = newBoneArray;
            Debug.Log("Copied bones successfully.");

        }
    }
}