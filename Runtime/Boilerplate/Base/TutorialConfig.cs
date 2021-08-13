using System;
using GamePack.TweenAlphaSetActive;

namespace Boilerplate.Base
{
    [Serializable]
    public struct TutorialConfig
    {
        public TutorialPanelConfig[] PanelConfigs;
    }

    [Serializable]
    public struct TutorialPanelConfig
    {
        public UIFader Panel;
        public float Delay;
        public float Duration;
    }
}