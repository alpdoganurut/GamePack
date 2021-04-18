using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class SkinnedMeshRendererInspector : MonoBehaviour
    {
        private SkinnedMeshRenderer _skinnedMeshRenderer;

        private SkinnedMeshRenderer SkinnedMeshRenderer
        {
            get
            {
                if (_skinnedMeshRenderer == null) _skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                return _skinnedMeshRenderer;
            }
        }

        [ShowInInspector] private Transform[] Bones => SkinnedMeshRenderer.bones;

        [Button, InfoBox("Copy bone information from a SkinnedMeshRenderer. Bones must have matching names.")]
        private void CopyBonesOfAnother(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            const string errorMessage = "Bones are not matching!";
            
            if (skinnedMeshRenderer.bones.Length != SkinnedMeshRenderer.bones.Length)
            {
                Debug.Log(errorMessage);
                return;
            }
            var newBoneArray = new Transform[SkinnedMeshRenderer.bones.Length];
            for (var index = 0; index < SkinnedMeshRenderer.bones.Length; index++)
            {
                var ourBone = SkinnedMeshRenderer.bones[index];
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

            SkinnedMeshRenderer.bones = newBoneArray;
            Debug.Log("Copied bones successfully.");

        }
    }
}