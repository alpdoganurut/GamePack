using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePack
{
    public class RandomGraphics: MonoBehaviour
    {
        [SerializeField, Required] private GameObject[] _Graphics;
        private void Awake()
        {
            RandomizeGraphics();
        }

        private void RandomizeGraphics()
        {
            var rng = Random.Range(0, _Graphics.Length);
            for (var index = 0; index < _Graphics.Length; index++)
            {
                var graphic = _Graphics[index];
                graphic.SetActive(index == rng);
            }
        }
    }
}