using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.PoolingSystem
{
    public class PoolController : MonoBehaviour
    {
        [SerializeField] private PoolableBase _Prefab;

        [ShowInInspector, ReadOnly] private readonly Stack<PoolableBase> _poolStack = new Stack<PoolableBase>();
        [ShowInInspector, ReadOnly] private readonly List<PoolableBase> _activeList = new List<PoolableBase>();
        [SerializeField] private int _PreFillCount;
        [SerializeField] private bool _IsPrefillInEditor;

        public List<PoolableBase> ActiveList => _activeList;

        private void Awake()
        {
            if(_IsPrefillInEditor)
                Prefill();
        }

        public void Init(PoolableBase poolablePrefab, int prefillCount)
        {
            _Prefab = poolablePrefab;
            _PreFillCount = prefillCount;
            Prefill();
        }

        private void Prefill()
        {
            var objs = new PoolableBase[_PreFillCount];
            for (var i = 0; i < _PreFillCount; i++)
            {
                objs[i] = FetchNew();
                PoolObject(objs[i]);
            }
        }

        public PoolableBase Get()
        {
            var newObj = FetchNew();
            newObj.OnStart();
            ActiveList.Add(newObj);
            return newObj;
        }
        
        /// Grab one from pool if available or create.  
        private PoolableBase FetchNew()
        {
            PoolableBase poolable;
            if (_poolStack.Count > 0)
            {
                poolable = _poolStack.Pop();
                // return _poolStack.Pop();
            }
            else
            {
                var newObj = Instantiate(_Prefab);
                poolable = newObj;
                poolable.LifeDidEnd += PoolObject;
                // return newObj;
            }
            
            return poolable;
        }

        private void PoolObject(PoolableBase obj)
        {
            _poolStack.Push(obj);
            // obj.LifeDidEnd -= PoolObject;
            obj.OnStop();
            _activeList.Remove(obj);
        }

        private void OnDestroy()
        {
            foreach (var poolableBase in ActiveList)
            {
                poolableBase.LifeDidEnd -= PoolObject;
            }
        }
    }
}