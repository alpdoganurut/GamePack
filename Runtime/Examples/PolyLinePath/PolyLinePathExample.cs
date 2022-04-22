using GamePack.TimerSystem;
using GamePack.Tools.Helper;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePack.Examples
{
    public class PolyLinePathExample : MonoBehaviour
    {
        [SerializeField] private PolyLinePath _Path;
        [SerializeField] private PolyLinePathFollower _Follower;

        private void Start()
        {
            var pathLength = _Path.TotalLength;
            _Follower.SetPathAndPos(_Path, 0);

            new Operation("PolyLinePathExample", duration: 2, updateAction: val =>
            {
                _Follower.PathPos = val * pathLength;

            }).Start().Repeat();
        }
    }
}