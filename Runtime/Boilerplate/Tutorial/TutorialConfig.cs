using System;
using GamePack.TweenAlphaSetActive;

// ReSharper disable InconsistentNaming

namespace GamePack.Boilerplate.Tutorial
{
    [Serializable]
    public struct TutorialConfig
    {
        public TutorialPanelConfig[] PanelConfigs;
    }

    [Serializable]
    public struct TutorialPanelConfig
    {
        public int LevelIndex;
        public UIFader Panel;
        public float Delay;
        public float Duration;
    }
}