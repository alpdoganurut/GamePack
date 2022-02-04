using UnityEngine;

namespace GamePack.Boilerplate.Main
{
    public class ConfigBase : ScriptableObject
    {
        // Shared config among projects
        [SerializeField, Range(.5f, 1.5f)] private float _DefaultTimeScale = 1;
        public float DefaultTimeScale => _DefaultTimeScale;
    }
}