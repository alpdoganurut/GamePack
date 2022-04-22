using System;
using System.Diagnostics.CodeAnalysis;
using GamePack.TweenAlphaSetActive;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Boilerplate
{
    public class GameEvents: MonoBehaviour
    {
        public enum SuccessRequirement
        {
            EnableIfSuccess, EnableIfFail, NotAffected
        }
        
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public struct GameEvent
        {
            public GameObject GameObject;
            [TableColumnWidth(30, resizable:false), DisableIf("@IsSuccess != SuccessRequirement.NotAffected")] 
            public bool IsEnabled; 
            [TableColumnWidth(120, resizable:false)] public SuccessRequirement IsSuccess;
            [TableColumnWidth(50, resizable:false)] public float Delay;
        }

        [SerializeField, TableList] private GameEvent[] _InitialEvents;
        [SerializeField, TableList] private GameEvent[] _StartGameEvents;
        [SerializeField, TableList] private GameEvent[] _EndGameEvents;

        private void Start()
        {
            foreach (var gameEvent in _InitialEvents)
            {
                var uiFader = gameEvent.GameObject.GetComponent<UIFader>();
                if (uiFader) uiFader.SetIsActive(gameEvent.IsEnabled);
                else gameEvent.GameObject.SetActive(gameEvent.IsEnabled);
            }
        }

        public void Trigger(bool isGameStart, bool? isSuccess = null)
        {
            var events = isGameStart ? _StartGameEvents : _EndGameEvents;
            foreach (var gameEvent in events)
            {
                var fader = gameEvent.GameObject.GetComponent<UIFader>();
                var eventGameobject = gameEvent.GameObject ? gameEvent.GameObject : fader.gameObject;
                LeanTween.cancel(eventGameobject);
                LeanTween.delayedCall(eventGameobject, gameEvent.Delay, () =>
                {
                    var isSuccessEnabled = gameEvent.IsSuccess == SuccessRequirement.NotAffected ||
                                         isSuccess == (gameEvent.IsSuccess == SuccessRequirement.EnableIfSuccess);
                    
                    var isEnabled = gameEvent.IsEnabled && isSuccessEnabled;
                    if(fader)
                        fader.SetIsActive(isEnabled);
                    else 
                        gameEvent.GameObject.SetActive(isEnabled);
                });
            }
        }
        
        #region Development
#if UNITY_EDITOR
        private void OnValidate()
        {
            void Validate(int index, GameEvent[] array)
            {
                var initialEvent = array[index];
                var newEvent = initialEvent;
                if (initialEvent.IsSuccess != SuccessRequirement.NotAffected) newEvent.IsEnabled = true;
                array[index] = newEvent;
            }

            for (var index = 0; index < _InitialEvents.Length; index++)
            {
                Validate(index, _InitialEvents);
            }
            for (var index = 0; index < _StartGameEvents.Length; index++)
            {
                Validate(index, _StartGameEvents);
            }
            for (var index = 0; index < _EndGameEvents.Length; index++)
            {
                Validate(index, _EndGameEvents);
            }
        }
#endif
        #endregion
    }
}