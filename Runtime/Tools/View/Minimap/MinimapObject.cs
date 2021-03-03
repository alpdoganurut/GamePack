using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Minimap
{
    public class MinimapObject: MonoBehaviour
    {
        [SerializeField, Required] private int _MinimapId;
        [SerializeField] private bool _PreLoaded;

        private void Start()
        {
            if(_PreLoaded) OnEnable();
        }

        public void OnEnable()
        {
            MinimapBase.GetById(_MinimapId)?.AddMapObject(this);
        }

        private void OnDisable()
        {
            MinimapBase.GetById(_MinimapId)?.RemoveMapObject(this);
        }
    }
}