using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Poolable
{
    public class PoolController : MonoBehaviour
    {
        [SerializeField] private PoolableBase _Prefab;

        [ShowInInspector, ReadOnly] private readonly Stack<PoolableBase> _poolStack = new Stack<PoolableBase>();
        [ShowInInspector, ReadOnly] private readonly List<PoolableBase> _activeList = new List<PoolableBase>();

        public List<PoolableBase> ActiveList => _activeList;

        public PoolableBase Get()
        {
            var newObj = FetchNew();
            newObj.LifeDidEnd += PoolObj;
            newObj.OnStart();
            ActiveList.Add(newObj);
            return newObj;
        }
        
        private PoolableBase FetchNew()
        {
            if (_poolStack.Count > 0)
            {
                return _poolStack.Pop();
            }
            else
            {
                var newObj = Instantiate(_Prefab);
                return newObj;
            }
        }

        private void PoolObj(PoolableBase obj)
        {
            _poolStack.Push(obj);
            obj.LifeDidEnd -= PoolObj;
            obj.OnStop();
            _activeList.Remove(obj);
        }

        private void OnDestroy()
        {
            foreach (var poolableBase in ActiveList)
            {
                poolableBase.LifeDidEnd -= PoolObj;
            }
        }
    }
}