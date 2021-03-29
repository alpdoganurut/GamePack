using UnityEngine;
// ReSharper disable InconsistentNaming

namespace HexGames
{
    // [CreateAssetMenu(fileName = "GameConfig", menuName = "Hex/Game Config", order = 0)]
    public class ConfigBase : ScriptableObject
    {
        [HideInInspector] public string GameUri = "com.hex.game";
    }
}