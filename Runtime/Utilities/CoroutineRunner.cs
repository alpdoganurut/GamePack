using System;
using System.Collections;
using UnityEngine;

namespace GamePack.UnityUtilities
{
    public class CoroutineRunner: MonoBehaviour
    {
        private static CoroutineRunner _runner;

        public static CoroutineRunner Runner
        {
            get
            {
                if(!_runner) _runner = new GameObject($"{typeof(CoroutineRunner)} - Instance").AddComponent<CoroutineRunner>();
                return _runner;
            }
        }

        public static Coroutine Run(IEnumerator coroutine)
        {
            return Runner.StartCoroutine(coroutine);
        }

        public static void RunMultiple(Action callback, params IEnumerator[] coroutines)
        {
            Runner.StartCoroutine(RunMultipleRoutine(callback, coroutines));
        }

        public static void Stop(Coroutine coroutine)
        {
            if(_runner)
                _runner.StopCoroutine(coroutine);
        }
        
        private static IEnumerator RunMultipleRoutine(Action callback, params IEnumerator[] coroutines)
        {
            foreach (var enumerator in coroutines)
            {
                yield return enumerator;
            }
            
            callback?.Invoke();
        }
        
    }
}