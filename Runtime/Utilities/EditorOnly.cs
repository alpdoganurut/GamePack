#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GamePack.Utilities
{
    [ExecuteAlways]
    public class EditorOnly: MonoBehaviour
    {
        private readonly string _title = $"{nameof(EditorOnly)}";
        private const string Question = "Revert this gameobject to normal state?";
        private const string Prefix = "[EditorOnly]";
        private const string RegexPrefix = @"\[EditorOnly\]";
        
        private void Awake()
        {
            gameObject.tag = "EditorOnly";
            
            if (!name.Contains(Prefix)) name = $"{Prefix} {name}";

            if(Application.isPlaying)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if(Application.isPlaying) return;
            if (!EditorUtility.DisplayDialog(_title, Question, "Revert", "Keep")) return;
            
            gameObject.tag = "Untagged";
            name = Regex.Replace(name, @$"{RegexPrefix}\s+", "");
        }
    }
}
#endif