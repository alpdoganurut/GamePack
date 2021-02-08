// SRDebugger Options. Duplicate this into project to use. 

// ReSharper disable once InconsistentNaming
public partial class SROptions
{
     // SAMPLES
    /*
     
     public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    
    public float InputSpeed
    {
        get => Game.Config.InputSpeed;
        set => Game.Config.InputSpeed = value;
    }
    
    [Category("Testing")]
    public void ShowTutorial()
    {
        var tutorialHandler = Object.FindObjectOfType<UITutorialHandler>();
        tutorialHandler.Show();
    }

    public void AdvanceLevel()
    {
        var levelHandler = Object.FindObjectOfType<LevelHandler>();
        levelHandler.StartCoroutine(levelHandler.DEV_AdvanceLevel());

        var mobs = Object.FindObjectsOfType<Monster>();
        foreach (var monster in mobs)
        {
            monster.Die();
        }
    }

    [Category("Testing")]
    public bool IsGodMode
    {
        get => MainVariables.Instance.GodMode;
        set
        {
            MainVariables.Instance.GodMode = value;
            Object.FindObjectOfType<ComboBonusHandler>().SetGodMode(value);
        }
    }

    [Category("Testing")]
    public int DevStartLevelIndex
    {
        get => MainVariables.Instance.DevStartLevelIndex;
        set => MainVariables.Instance.DevStartLevelIndex = value;
    }

    // --- GENERAL --- 
    
    [Category("General")]
    public float Sensitivity
    {
#if UNITY_EDITOR
        get => MainVariables.Instance.EditorGamepadEnabled ?
            MainVariables.Instance.GamepadSensitivity :  MainVariables.Instance.TouchSensitivity;
        set
        {
            if (MainVariables.Instance.EditorGamepadEnabled)
                MainVariables.Instance.GamepadSensitivity = value;
            else
                MainVariables.Instance.TouchSensitivity = value;
        }
#elif UNITY_STANDALONE
        get => MainVariables.Instance.GamepadSensitivity;
        set => MainVariables.Instance.GamepadSensitivity = value;
#elif UNITY_IOS || UNITY_ANDROID
        set => MainVariables.Instance.TouchSensitivity = value;
        get => MainVariables.Instance.TouchSensitivity;
#endif
    }
    
    [Category("General")]
    public float BetweenLevelDelay
    {
        get => MainVariables.Instance.BetweenLevelDelay;
        set => MainVariables.Instance.BetweenLevelDelay = value;
    }

    [Category("General")]
    public int Seed
    {
        get => MainVariables.Instance.Seed;
        set => MainVariables.Instance.Seed = value;
    }
    
    [Category("General")]
    public bool TimeSlowWhenGettingAttacked
    {
        get => MainVariables.Instance.TimeSlowWhenGettingAttacked;
        set => MainVariables.Instance.TimeSlowWhenGettingAttacked = value;
    }
    [Category("General")]
    public float TimeSlowWhenGettingAttackedTimeScale
    {
        get => MainVariables.Instance.TimeSlowWhenGettingAttackedTimeScale;
        set => MainVariables.Instance.TimeSlowWhenGettingAttackedTimeScale = value;
    }

    // --- MECHANICS --- 

    [Category("Mechanics")]
    public float ComboTimeLimit
    {
        get => MainVariables.Instance.ComboTimeLimit;
        set => MainVariables.Instance.ComboTimeLimit = value;
    }

    // --- UI --- 

    [Category("UI")]
    public float ComboIndicatorScaleDuration
    {
        get => Game.Instance.BonusHandler.IndicatorScaleAnimationDuration;
        set => Game.Instance.BonusHandler.IndicatorScaleAnimationDuration = value;
    }

    [Category("UI")]
    public float BonusWobbleAmountOnHit
    {
        get => MainVariables.Instance.BonusWobbleAmountOnHit;
        set => MainVariables.Instance.BonusWobbleAmountOnHit = value;
    }

    [Category("UI")]
    public float BonusWobbleAmountOnFill
    {
        get => MainVariables.Instance.BonusWobbleAmountOnFill;
        set => MainVariables.Instance.BonusWobbleAmountOnFill = value;
    }

    [Category("UI")]
    public float BonusWobbleAmountOnLevelChange
    {
        get => MainVariables.Instance.BonusWobbleAmountOnLevelChange;
        set => MainVariables.Instance.BonusWobbleAmountOnLevelChange = value;
    }

    // --- PLAYER --- 

    [Category("Player Bounce")]
    public float PlayerBounceTimeScaleForSeconds
    {
        get => MainVariables.Instance.PlayerBounceTimeScaleForSeconds;
        set => MainVariables.Instance.PlayerBounceTimeScaleForSeconds = value;
    }

    [Category("Player Bounce")]
    public float PlayerBounceTimeScale
    {
        get => MainVariables.Instance.PlayerBounceTimeScale;
        set => MainVariables.Instance.PlayerBounceTimeScale = value;
    }

    [Category("Player Bounce")]
    public bool PlayerBounceSlowMotion
    {
        get => MainVariables.Instance.PlayerBounceSlowMotion;
        set => MainVariables.Instance.PlayerBounceSlowMotion = value;
    }

    [Category("Player Bounce")]
    public float PlayerBounceMaxDelay
    {
        get => MainVariables.Instance.PlayerBounceMaxDelay;
        set => MainVariables.Instance.PlayerBounceMaxDelay = value;
    }

    [Category("Player Bounce")]
    public float PlayerBounceMinDelay
    {
        get => MainVariables.Instance.PlayerBounceMinDelay;
        set => MainVariables.Instance.PlayerBounceMinDelay = value;
    }

    [Category("Player")]
    public float PlayerMoveDuration
    {
        get => MainVariables.Instance.PlayerMoveDuration;
        set => MainVariables.Instance.PlayerMoveDuration = value;
    }

    [Category("Player")]
    public float GameEntityReturnDuration
    {
        get => MainVariables.Instance.GameEntityReturnDuration;
        set => MainVariables.Instance.GameEntityReturnDuration = value;
    }

    [Category("Player")]
    public bool PlayerCanStartAnywhere
    {
        get => MainVariables.Instance.PlayerCanStartAnywhere;
        set => MainVariables.Instance.PlayerCanStartAnywhere = value;
    }

    // --- MONSTER --- 

    [Category("Monster")]
    public bool IsRandomMonsterMoveInterval
    {
        get => MainVariables.Instance.IsRandomMonsterMoveInterval;
        set => MainVariables.Instance.IsRandomMonsterMoveInterval = value;
    }

    [Category("Monster")]
    public float MonsterSpikeOutDuration
    {
        get => MainVariables.Instance.SpikeOutDuration;
        set => MainVariables.Instance.SpikeOutDuration = value;
    }
    
    [Category("Mechanics")]
    public float TouchDirectionalMoveDifference
    {
        get => MainVariables.Instance.TouchDirectionalMoveDifference;
        set => MainVariables.Instance.TouchDirectionalMoveDifference = value;
    }*/
}