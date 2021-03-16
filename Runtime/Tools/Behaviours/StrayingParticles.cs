using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePack
{
    [RequireComponent(typeof(ParticleSystem))]
    public class StrayingParticles : MonoBehaviour
    {
        
        private const float ParticleRotLerpSpeed = 5f;

        private ParticleSystem _particleSystem;

        private ParticleSystem ParticleSystem
        {
            get
            {
                if (!_particleSystem) _particleSystem = GetComponent<ParticleSystem>();
                return _particleSystem;
            }
        }
        
        [SerializeField, Required] private bool _IsStray = true;
        [SerializeField] private float _StrayVelMagnitude = .1f;
        [SerializeField] private float _StrayChance = .001f;
        [SerializeField] private Vector3 _ParticleRotationOffset = new Vector3(-90, 0, 0);

        
        private void LateUpdate()
        {
            var particleArr = new ParticleSystem.Particle[ParticleSystem.particleCount];
            ParticleSystem.GetParticles(particleArr);

            for (var index = 0; index < particleArr.Length; index++)
            {
                var particle = particleArr[index];
                var particleVelocity = particle.velocity;

                // Passive particles strays randomly
                if (_IsStray && Random.Range(0, 1f) < _StrayChance)
                {
                    var randomDir = Random.onUnitSphere;
                    randomDir.y = 0;
                    randomDir *= Random.Range(.5f, 1f) * _StrayVelMagnitude;

                    particleVelocity = randomDir;
                }
                
                if (particleVelocity.sqrMagnitude > 0.001f)
                {
                    var targetRotation = (Quaternion.LookRotation(particleVelocity) *
                                          Quaternion.Euler(_ParticleRotationOffset));
                    particle.rotation3D = Quaternion.Lerp(Quaternion.Euler(particle.rotation3D), targetRotation,
                        ParticleRotLerpSpeed * Time.deltaTime).eulerAngles;
                }

                particle.velocity = particleVelocity;
                particleArr[index] = particle;
            }
            _particleSystem.SetParticles(particleArr, particleArr.Length);
        }
    }
}