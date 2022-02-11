using GamePack.Minimap;
using GamePack.Poolable;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Examples.Minimap
{
    public class CustomMinimap : MinimapBase
    {
        [SerializeField, Required] private PoolController _IndicatorPool;
        protected override PoolableIndicator GetPoolable(MinimapObject minimapObject)
        {
            return _IndicatorPool.Get() as PoolableIndicator;
        }
    }
}
