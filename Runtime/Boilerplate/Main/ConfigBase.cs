using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Boilerplate.Main
{
    public class ConfigBase : ScriptableObject
    {
        // Shared config among projects
        [SerializeField, HideInInspector] private bool _AutoEnterMainScene;
        [SerializeField, Range(.5f, 1.5f)] private float _DefaultTimeScale = 1;
        
        public float DefaultTimeScale => _DefaultTimeScale;

        public bool AutoEnterMainScene
        {
            get => _AutoEnterMainScene;
            set => _AutoEnterMainScene = value;
        }
    }
}