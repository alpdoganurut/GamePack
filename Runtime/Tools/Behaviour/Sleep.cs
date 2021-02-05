using System;
using UnityEngine;

namespace GamePack
{
    public class Sleep
    {
        public event Action<bool> SleepFunc;  
        
        private float _sleepEnd;
        
        public bool IsSleep => Time.time < _sleepEnd;
        
        public void SetSleep(float seconds)
        {
            _sleepEnd = Time.time + seconds;
            SleepFunc?.Invoke(true);
            LeanTween.delayedCall(seconds, () =>
            {
                SleepFunc?.Invoke(false);
            });
        }
    }
}