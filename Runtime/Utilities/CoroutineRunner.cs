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

        public static void Run(IEnumerator coroutine)
        {
            Runner.StartCoroutine(coroutine);
        }

        public static void RunMultiple(Action callback, params IEnumerator[] coroutines)
        {
            Runner.StartCoroutine(RunMultipleRoutine(callback, coroutines));
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