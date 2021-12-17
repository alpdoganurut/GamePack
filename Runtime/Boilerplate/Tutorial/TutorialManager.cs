using System;
using Boilerplate.Base;
using GamePack.TweenAlphaSetActive;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace GamePack
{
    public class TutorialManager: MonoBehaviour
    {
        [SerializeField, ReadOnly] private string _TutorialPrefsKey;
        [SerializeField, Required] private TutorialConfig _TutorialConfig;

        private UIFader _activePanel;

        public void ShowTutorial(int index)
        {
            
            if(_TutorialConfig.PanelConfigs == null || index >= _TutorialConfig.PanelConfigs.Length) return;
            
            var panelConfig = _TutorialConfig.PanelConfigs[index];
            
            LeanTween.cancel(gameObject);
            
            if(IsTutorialShown(index)) return;
            if(_activePanel) _activePanel.SetIsActive(false);

            var tutorialPanel = panelConfig.Panel;
            
            _activePanel = tutorialPanel;
            
            SetTutorialShown(index, true);

            // Show with delay
            LeanTween.delayedCall(gameObject, panelConfig.Delay, () =>
            {
                tutorialPanel.SetIsActive(true);
            });
            // Hide
            LeanTween.delayedCall(gameObject, panelConfig.Delay + panelConfig.Duration, () =>
            {
                tutorialPanel.SetIsActive(false);
                _activePanel = null;
            });
        }

        private bool IsTutorialShown(int index)
        {
            return PlayerPrefs.GetInt(GetTutorialKey(index)) > 0;
        }

        private void SetTutorialShown(int index, bool isShown)
        {
            PlayerPrefs.SetInt(GetTutorialKey(index), isShown ? 1 : 0);
        }

        private string GetTutorialKey(int index)
        {
            return _TutorialPrefsKey + index;
        }

        [Button]
        private void ResetTutorial()
        {
            for (var index = 0; index < _TutorialConfig.PanelConfigs.Length; index++)
            {
                SetTutorialShown(index, false);
            }
        }
        
        #region Development
#if UNITY_EDITOR
        private void OnValidate()
        {
            _TutorialPrefsKey = PlayerSettings.applicationIdentifier + ".tutorial";
            
            foreach (var panelConfig in _TutorialConfig.PanelConfigs)
            {
                panelConfig.Panel.DisableOnStart = true;
            }
        }


#endif
        #endregion
    }
}