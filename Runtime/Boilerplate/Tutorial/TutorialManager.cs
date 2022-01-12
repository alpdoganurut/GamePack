using System.Collections.Generic;
using System.Linq;
using Boilerplate.Base;
using GamePack.Timer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    public class TutorialManager: MonoBehaviour
    {
        [SerializeField, Required] private TutorialConfig _TutorialConfig;

        private readonly List<OperationTreeDescription> _operations = new List<OperationTreeDescription>();

        public void ShowTutorial(int levelIndex)
        {
            if(_TutorialConfig.PanelConfigs == null || levelIndex >= _TutorialConfig.PanelConfigs.Length) return;

            var panelConfigs = _TutorialConfig.PanelConfigs.Where(config => config.LevelIndex == levelIndex);

            foreach (var config in panelConfigs)
            {
                var op = new Operation(delay: config.Delay, duration: config.Duration,
                    action: () =>
                    {
                        config.Panel.SetIsActive(true);
                    },
                    endAction: () =>
                    {
                        config.Panel.SetIsActive(false);
                    }).Start();
                
                _operations.Add(op);
            }
        }

        public void Cancel()
        {
            foreach (var operation in _operations)
            {
                operation.Cancel();
            }
            _operations.Clear();
        }
        
        #region Development
#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var panelConfig in _TutorialConfig.PanelConfigs)
            {
                if(panelConfig.Panel) panelConfig.Panel.DisableOnStart = true;
            }
        }
#endif
        #endregion
    }
}