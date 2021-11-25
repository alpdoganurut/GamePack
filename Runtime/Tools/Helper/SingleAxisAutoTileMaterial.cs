using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TrickyHands
{
    [RequireComponent(typeof(MeshRenderer))]
    [ExecuteInEditMode]
    public class SingleAxisAutoTileMaterial: MonoBehaviour
    {
        #region Req. Component MeshRenderer

        private MeshRenderer _meshRenderer;

        private MeshRenderer MeshRenderer
        {
            get
            {
                if (!_meshRenderer) _meshRenderer = GetComponent<MeshRenderer>();
                return _meshRenderer;
            }
        }

        #endregion

        private enum Axis
        {
            X, Y, Z
        }
        
        [SerializeField, DisableIf("_isInit")] private int _MaterialIndex = 0;
        [SerializeField, DisableIf("_isInit"), HorizontalGroup("Axis")] private Axis _ScaleAxis;
        [SerializeField, DisableIf("_isInit"), HorizontalGroup("Axis")] private Axis _MaterialTileAxis;
        [SerializeField, HideInInspector] private float _ScaleToTileFactor = 1;
        [SerializeField, HideInInspector] private Material _SharedMaterial;
        [SerializeField, HideInInspector] private Material _OriginalMaterial;
        
        private Material Material
        {
            get => MeshRenderer.sharedMaterials[_MaterialIndex];
            set
            {
                var newMaterials = MeshRenderer.sharedMaterials;
                newMaterials[_MaterialIndex] = value;
                MeshRenderer.sharedMaterials = newMaterials;
            }
        }
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
        private Vector3 _lastScale;
        private bool _isInit;
        private bool _updateWhenPlaying;

        private void Update()
        {
            if(Application.isPlaying && !_updateWhenPlaying) return;
            if (!_isInit) return;
            if (_lastScale != transform.lossyScale)
            {
                UpdateMaterialTiling();
            }
            
            _lastScale = transform.lossyScale;
        }

        [Button(ButtonSizes.Large), ShowIf("_isInit")]
        private void Rollback()
        {
            if (!_OriginalMaterial && EditorUtility.DisplayDialog("AutoTile",
                $"Can't find original material, still want to rollback?", "Don't rollback", "Yes"))
            {
                return;
            }
            
            Material = _OriginalMaterial;
            _SharedMaterial = null;
            _isInit = false;
        }
        
        [Button(ButtonSizes.Large), HideIf("_isInit")]
        private void Init()
        {
            if (!Material)
            {
                Debug.LogError($"No material found at index {_MaterialIndex}. Can't initiate Auto Tile.");
                return;
            }
            
            GetScaleToTileFactor();
            CheckAndCreateMaterial();
            UpdateMaterialTiling();
            _isInit = true;
        }
        
        private void CheckAndCreateMaterial()
        {
            if (_SharedMaterial) return;

            _OriginalMaterial = Material;
            var newMatName = _OriginalMaterial.name;
            newMatName = $"{newMatName}_{name}_AutoTile";

            var sourcePath = AssetDatabase.GetAssetPath(_OriginalMaterial);
            var folderPath = "AutoTileDuplicate";
            var targetPath = $"Assets/{folderPath}/{newMatName}.mat";

            if (EditorUtility.DisplayDialog("AutoTile",
                $"AutoTile will create a new material at {targetPath}", "OK", "Remove Auto Tile"))
            {
                if (!AssetDatabase.IsValidFolder($"Assets/{folderPath}"))
                {
                    AssetDatabase.CreateFolder("Assets", folderPath);
                }

                if (!AssetDatabase.CopyAsset(sourcePath, targetPath))
                {
                    Debug.LogWarning($"Failed to copy {sourcePath}");
                    Fail();
                }

                var newMaterial = AssetDatabase.LoadAssetAtPath<Material>(targetPath);
                _SharedMaterial = newMaterial;
                Material = newMaterial;

            }
            else Fail();
        }

        private void GetScaleToTileFactor()
        {
            var scaleVal = GetScaleValue();
            var tileVal = GetTileValue();
            _ScaleToTileFactor = tileVal / scaleVal;
        }

        private void UpdateMaterialTiling()
        {
            if(!_SharedMaterial)
            {
                Debug.LogError("No shared material exists! This should not happen.");
                return;
            }
            
            var scaleVal = GetScaleValue();

            var tileVal = scaleVal * _ScaleToTileFactor;
            var currentTile  = Material.GetTextureScale(BaseMap);
            
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (_MaterialTileAxis)
            {
                case Axis.X:
                    Material.SetTextureScale(BaseMap, new Vector2(tileVal, currentTile.y));
                    break;
                case Axis.Y:
                    Material.SetTextureScale(BaseMap, new Vector2(currentTile.x, tileVal));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private float GetScaleValue()
        {
            var scaleVal = 0f;
            switch (_ScaleAxis)
            {
                case Axis.X:
                    scaleVal = transform.lossyScale.x;
                    break;
                case Axis.Y:
                    scaleVal = transform.lossyScale.y;
                    break;
                case Axis.Z:
                    scaleVal = transform.lossyScale.z;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return scaleVal;
        }
        
        private float GetTileValue()
        {
            var scaleVal = 1f;
            switch (_MaterialTileAxis)
            {
                case Axis.X:
                    scaleVal = Material.GetTextureScale(BaseMap).x;
                    break;
                case Axis.Y:
                    scaleVal = Material.GetTextureScale(BaseMap).y;
                    break;
                case Axis.Z:
                    Debug.LogError("Can't get Z Axis for tiling.");
                    // scaleVal = transform.lossyScale.z;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return scaleVal;
        }
        
        private void Fail()
        {
            DestroyImmediate(this);
        }
    }
}