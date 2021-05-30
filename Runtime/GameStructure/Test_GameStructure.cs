using UnityEngine;

namespace GameStructure
{
    public class Test_GameStructure: MonoBehaviour
    {
        private LogTimeResponder _tester;

        private void Awake()
        {
            _tester = new LogTimeResponder();
        }
    }
}