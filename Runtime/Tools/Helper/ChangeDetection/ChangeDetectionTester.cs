#region Development
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Tools.Helper
{
    public class ChangeDetectionTester: MonoBehaviour
    {
        [SerializeField, Required] private bool _IsDetect;
        [SerializeField, Required] private bool _IsActive = true;
        [SerializeField, Required] private float _Delay;

        private void Awake()
        {
            var _ = new ChangeDetection(() => _IsDetect, isDetect => Debug.Log(isDetect), () => _IsActive, () => _Delay);
        }
    }
} 
#endif
#endregion