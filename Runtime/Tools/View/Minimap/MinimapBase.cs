using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace GamePack.Minimap
{
    public abstract class MinimapBase: MonoBehaviour
    {
        #region Static

        private static readonly Dictionary<int, MinimapBase> Minimaps = new Dictionary<int, MinimapBase>();
        public static MinimapBase GetById(int id){return Minimaps.ContainsKey(id) ? Minimaps[id] : null; }
        
        #endregion
        
        /// <summary>
        /// Used to support multiple minimaps if necessary, mostly for the sake of good structure.
        /// </summary>
        [SerializeField, Required] private int _ID;
        [SerializeField, Required] private float _MapScale = 5f;
        
        [FormerlySerializedAs("_PlayerBoat")] 
        [SerializeField, Required] private Transform _CentralObject;
        [SerializeField, Required] private Transform _EnvironmentCenter;
        
        [FormerlySerializedAs("_MapBackground")]
        [SerializeField, Required] private RectTransform _MapIndicatorWrapper;
        [FormerlySerializedAs("_EnvironmentObject")]
        [SerializeField, Required] private RectTransform _MapBackgroundImage;
        
        [ShowInInspector, ReadOnly] private Dictionary<MinimapObject, PoolableIndicator> _indicatorsDictionary = new Dictionary<MinimapObject, PoolableIndicator>();
        
        private void Awake()
        {
            Assert.IsFalse(Minimaps.ContainsKey(_ID));
            
            Minimaps.Add(_ID, this);

            // InitializeSceneMinimapObjects();
            // _MapArea.GetWorldCorners(_mapAreaCorners);
        }

        private void InitializeSceneMinimapObjects()
        {
            var sceneMinimapObjects = FindObjectsOfType<MinimapObject>();
            foreach (var sceneMinimapObject in sceneMinimapObjects)
            {
                sceneMinimapObject.OnEnable();
            }
        }

        private void Update()
        {
            UpdateGraphics();
        }

        public void AddMapObject(MinimapObject minimapObject)
        {
            var newIndicator = GetPoolable(minimapObject);

            newIndicator.transform.SetParent(_MapIndicatorWrapper, false);
            _indicatorsDictionary.Add(minimapObject, newIndicator);
        }

        private void UpdateGraphics()
        {
            var environmentOffset = _CentralObject.InverseTransformPoint(_EnvironmentCenter.position) * _MapScale;
            _MapBackgroundImage.anchoredPosition = new Vector3(environmentOffset.x, environmentOffset.z);
            
            foreach (var keyValue in _indicatorsDictionary)
            {
                var mapObject = keyValue.Key;
                var indicator = keyValue.Value;

                // If map object doesn't exist disable it
                if (!mapObject || !mapObject.gameObject.activeInHierarchy)
                {
                    RemoveMapObject(mapObject);
                    return;
                }
                
                var mapObjectTransform = mapObject.transform;
                
                var offset = _CentralObject.InverseTransformPoint(mapObjectTransform.position) * _MapScale;
                var rotation = mapObjectTransform.localRotation * Quaternion.Inverse(_CentralObject.rotation); 
                
                indicator.RectTransform.anchoredPosition = new Vector3(offset.x, offset.z);
                indicator.transform.localRotation = Quaternion.Euler(0, 0, -rotation.eulerAngles.y);
            }
        }

        public void RemoveMapObject(MinimapObject mapObject)
        {
            if(!_indicatorsDictionary.ContainsKey(mapObject)) return;
            
            var indicator = _indicatorsDictionary[mapObject];
            _indicatorsDictionary.Remove(mapObject);
            indicator.EndLife();
        }

        protected abstract PoolableIndicator GetPoolable(MinimapObject minimapObject);
    }
}