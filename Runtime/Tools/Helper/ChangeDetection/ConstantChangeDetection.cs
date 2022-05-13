using System;
using System.Collections;
using GamePack.UnityUtilities;
using GamePack.Utilities;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    public class ConstantChangeDetection
    {
        private readonly Func<bool> _condition;
        private readonly Action<bool> _action;
        private readonly Func<bool> _activeAction;
        
        private readonly Coroutine _coroutine;

        public ConstantChangeDetection(Func<bool> changeCondition, Action<bool> action, Func<bool> activeAction)
        {
            _condition = changeCondition;
            _action = action;
            _activeAction = activeAction;

            _coroutine = CoroutineRunner.Run(Update());
        }

        private IEnumerator Update()
        {
            while (true)
            {
                if (!_activeAction.Invoke())
                {
                    yield return true;
                    continue;
                }
                _action(_condition.Invoke());
                yield return true;
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        // TODO: This would not be called naturally since coroutine running operation should have a reference to this object 
        ~ConstantChangeDetection()
        {
            CoroutineRunner.Runner.StopCoroutine(_coroutine);
        }
    }
}