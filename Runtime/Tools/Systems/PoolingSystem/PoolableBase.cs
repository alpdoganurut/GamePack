using System;
using UnityEngine;

namespace GamePack.PoolingSystem
{
    public abstract class PoolableBase: MonoBehaviour 
    {
        public event Action<PoolableBase> LifeDidEnd;

        public void EndLife()
        {
            LifeDidEnd?.Invoke(this);
        }

        internal abstract void OnStart();
        internal abstract void OnStop();
    }
}