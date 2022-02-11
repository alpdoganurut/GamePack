using System;
using UnityEngine;

namespace GamePack.Poolable
{
    public abstract class PoolableBase: MonoBehaviour 
    {
        public event Action<PoolableBase> LifeDidEnd;

        public void EndLife()
        {
            LifeDidEnd?.Invoke(this);
        }

        public abstract void OnStart();
        public abstract void OnStop();
    }
}