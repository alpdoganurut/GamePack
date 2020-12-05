using System;
using System.Collections;
using GamePack.TweenAlphaSetActive;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace GamePack
{
    public class Countdown: MonoBehaviour
    {
        [SerializeField, Required] private CanvasGroupTweenAlphaSetActiveHandler _TextPanel;
        [SerializeField, Required] private TextMeshProUGUI _Text;
        
        private int _seconds;

        public void StartCountdown(int seconds, Action callback)
        {
            _seconds = seconds;
            StartCoroutine(Count(callback));
        }

        private IEnumerator Count(Action callback)
        {
            _TextPanel.SetIsActive(true);
            for (var i = 0; i < _seconds; i++)
            {
                _Text.text = (_seconds - i).ToString();
                yield return new WaitForSeconds(1);
            }
            _TextPanel.SetIsActive(false);
            callback?.Invoke();
        }
    }
}