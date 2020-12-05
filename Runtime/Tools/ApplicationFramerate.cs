using UnityEngine;

namespace GamePack
{
    public class ApplicationFramerate : MonoBehaviour
    {
        [SerializeField] private int _Framerate = 60;

        private void Start()
        {
            Application.targetFrameRate = _Framerate;
        }
    }
}