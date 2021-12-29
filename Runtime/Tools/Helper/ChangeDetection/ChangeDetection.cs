using System;
using System.Collections;
using GamePack.UnityUtilities;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    public class ChangeDetection
    {
        private readonly Func<bool> _changeCondition;
        private readonly Action<bool> _detectAction;
        private readonly Func<bool> _activeCondition;
        private readonly Func<float> _delay;
        
        private bool _wasDetecting;
        private float _detectionTime;
        private bool _isActed;
        private readonly Coroutine _coroutine;

        public bool IsActive { get; set; } = true;
        
        public ChangeDetection(Func<bool> changeCondition, Action<bool> detectAction, Func<bool> activeCondition = null, Func<float> delay = null)
        {
            _changeCondition = changeCondition;
            _delay = delay;
            _detectAction = detectAction;
            _activeCondition = activeCondition;
            
            _coroutine = CoroutineRunner.Run(Update());
        }
        
        private IEnumerator Update()
        {
            while (true)
            {
                if (!IsActive || !(_activeCondition?.Invoke() ?? true))
                {
                    yield return true;
                    continue;
                }
                
                var isDetecting = _changeCondition.Invoke();
                 if (!_wasDetecting && isDetecting)
                {
                    _detectionTime = Time.time;
                    _isActed = false;
                }
                else if (_wasDetecting && !isDetecting && _isActed)
                {
                    _detectAction?.Invoke(false);
                }
                else if ( !_isActed && isDetecting && _wasDetecting && Time.time >
                                                        _detectionTime + (_delay?.Invoke() ?? 0))
                {
                    _isActed = true;
                    _detectAction?.Invoke(true);
                }

                _wasDetecting = isDetecting;
                
                yield return true;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public void Destroy()
        {
            CoroutineRunner.Stop(_coroutine);
        }
    }
}