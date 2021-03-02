using System.Collections;
using System.Collections.Generic;
using GamePack.Minimap;
using GamePack.Poolable;
using Sirenix.OdinInspector;
using UnityEngine;

public class ExampleMinimap : MinimapBase
{
    [SerializeField, Required] private PoolController _IndicatorPool;
    protected override PoolableIndicator GetPoolable(MinimapObject minimapObject)
    {
        return _IndicatorPool.Get() as PoolableIndicator;
    }
}
