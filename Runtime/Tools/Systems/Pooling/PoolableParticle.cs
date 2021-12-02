using System;
using UnityEngine;

namespace GamePack.Poolable
{
    [RequireComponent(typeof(ParticleSystem))] 
    public class PoolableParticle : PoolableBase
    {
        private ParticleSystem _particleSystem;

        public ParticleSystem ParticleSystem
        {
            get
            {
                if (_particleSystem == null) _particleSystem = GetComponent<ParticleSystem>();
                return _particleSystem;
            }
        }
        
        private void OnParticleSystemStopped()
        {
            EndLife();
        }

        public override void OnStart()
        {
            gameObject.SetActive(true);
            ParticleSystem.Play();
        }

        public override void OnStop()
        {
            gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            var mainSettings = ParticleSystem.main;
            if (mainSettings.stopAction != ParticleSystemStopAction.Callback)
                mainSettings.stopAction = ParticleSystemStopAction.Callback;
        }
    }
}