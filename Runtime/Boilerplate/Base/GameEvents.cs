using System;
using System.Diagnostics.CodeAnalysis;
using GamePack.TweenAlphaSetActive;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace HexGames
{
    public class GameEvents: MonoBehaviour
    {
        public enum SuccessRequirement
        {
            EnableIf, DisableIf, NotAffected
        }
        
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public struct GameEvent
        {
            /*[TableColumnWidth(100)] */public GameObject GameObject;
            [TableColumnWidth(30, resizable:false)] public bool IsEnabled; 
            [TableColumnWidth(80, resizable:false)] public SuccessRequirement IsSuccess;
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
                    var isSuccessMatch = gameEvent.IsSuccess == SuccessRequirement.NotAffected ||
                                         isSuccess == (gameEvent.IsSuccess == SuccessRequirement.EnableIf);
                    
                    var isEnabled = gameEvent.IsEnabled && isSuccessMatch;
                    if(fader)
                        fader.SetIsActive(isEnabled);
                    else 
                        gameEvent.GameObject.SetActive(isEnabled);
                });
            }
        }
    }
}