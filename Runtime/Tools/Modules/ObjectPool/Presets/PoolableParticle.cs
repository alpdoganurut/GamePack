using UnityEngine;

namespace GamePack.Modules.ObjectPool
{
    [RequireComponent(typeof(ParticleSystem))] 
    public class PoolableParticle : PoolableBase
    {
        private ParticleSystem _particleSystem;

        public ParticleSystem ParticleSystem
        {   
            get
            {
                if (!_particleSystem) _particleSystem = GetComponent<ParticleSystem>();
                return _particleSystem;
            }
        }
        
        private void OnParticleSystemStopped() => EndLife();

        internal override void OnStart()
        {
            gameObject.SetActive(true);
            ParticleSystem.Play();
        }

        internal override void OnStop() => gameObject.SetActive(false);

        private void OnValidate()
        {
            var mainSettings = ParticleSystem.main;
            if (mainSettings.stopAction != ParticleSystemStopAction.Callback)
                mainSettings.stopAction = ParticleSystemStopAction.Callback;
        }
    }
}