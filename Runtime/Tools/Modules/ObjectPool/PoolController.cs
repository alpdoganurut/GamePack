using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Modules.ObjectPool
{
    public class PoolController<T> : MonoBehaviour where T: PoolableBase
    {
        [SerializeField] private T _Prefab;

        [ShowInInspector, ReadOnly] private readonly Stack<T> _poolStack = new Stack<T>();
        [ShowInInspector, ReadOnly] private readonly List<T> _activeList = new List<T>();
        [SerializeField] private int _PreFillCount;
        [SerializeField] private bool _InitializeAtAwake;

        public List<T> ActiveList => _activeList;

        private void Awake()
        {
            if(_InitializeAtAwake)
                Prefill();
        }

        public void Init(T poolablePrefab, int prefillCount)
        {
            _Prefab = poolablePrefab;
            _PreFillCount = prefillCount;
            Prefill();
        }

        public T Get()
        {
            var newObj = FetchNew();
            newObj.OnStart();
            ActiveList.Add(newObj);
            return newObj;
        }

        private void Prefill()
        {
            var objs = new T[_PreFillCount];
            for (var i = 0; i < _PreFillCount; i++)
            {
                objs[i] = FetchNew();
                PoolObject(objs[i]);
            }
        }

        /// Grab one from pool if available or create new.  
        private T FetchNew()
        {
            T poolable;
            if (_poolStack.Count > 0)
            {
                poolable = _poolStack.Pop();
            }
            else
            {
                var newObj = Instantiate(_Prefab);
                poolable = newObj;
                poolable.LifeDidEnd += PoolObject;
            }
            
            return poolable;
        }

        private void PoolObject(T obj)
        {
            _poolStack.Push(obj);
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