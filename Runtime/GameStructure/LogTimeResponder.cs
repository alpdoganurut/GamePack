using UnityEngine;

namespace GameStructure
{
    public class LogTimeResponder: TimeResponder
    {
        public LogTimeResponder()
        {
            Debug.Log($"Constructor: {nameof(LogTimeResponder)}");
        }

        public override void Start()
        {
            Debug.Log($"Start: {nameof(LogTimeResponder)}");
        }
        
        public override void Update()
        {
            Debug.Log($"Update: {nameof(LogTimeResponder)}");
        }
    }

}