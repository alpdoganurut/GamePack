using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Logging
{
    [CreateAssetMenu(fileName = "Managed Log Configuration", menuName = "GamePack/ManagedLogConfig", order = 0)]
    public class ManagedLogConfig : ScriptableObject
    {
        [SerializeField, Required] private ManagedLog.Type[] _LogTypes;

        [SerializeField, Required] private bool _ShowFrameCount = true;
        [SerializeField, Required] private bool _ShowLogType = true;
        public ManagedLog.Type[] LogTypes => _LogTypes;

        public bool ShowFrameCount => _ShowFrameCount;

        public bool ShowLogType => _ShowLogType;

        private void OnValidate()
        {
            // Make sure error exists
            if (Array.IndexOf(_LogTypes, ManagedLog.Type.Error) < 0)
            {
                var cloned = new ManagedLog.Type[_LogTypes.Length+1];
                Array.Copy(_LogTypes, 0, cloned, 1, _LogTypes.Length);
                cloned[0] = ManagedLog.Type.Error;
                _LogTypes = cloned;
            }
            
            // Make sure default exists
            if (Array.IndexOf(_LogTypes, ManagedLog.Type.Default) < 0)
            {
                var cloned = new ManagedLog.Type[_LogTypes.Length+1];
                Array.Copy(_LogTypes, 0, cloned, 1, _LogTypes.Length);
                cloned[0] = ManagedLog.Type.Default;
                _LogTypes = cloned;
            }

            // Increment last added Type
            var enumLength = Enum.GetNames(typeof(ManagedLog.Type)).Length;
            var offset = 0;
            while (_LogTypes.Count(type => type == _LogTypes[_LogTypes.Length - 1]) > 1 && offset < enumLength - 1)
            {
                offset++;
                _LogTypes[_LogTypes.Length - 1] =
                    (ManagedLog.Type) offset;
            }
            
            // Last check that no type repeats in array
            _LogTypes = _LogTypes.Distinct().ToArray();
        }
    }
}