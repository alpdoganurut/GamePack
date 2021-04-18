using Sirenix.OdinInspector;
using UnityEngine;
// ReSharper disable InconsistentNaming

namespace HexGames
{
    // [CreateAssetMenu(fileName = "GameConfig", menuName = "Hex/Game Config", order = 0)]
    public class ConfigBase : ScriptableObject
    {
        [SerializeField, Required] public string WorkingTitle;
    }
}