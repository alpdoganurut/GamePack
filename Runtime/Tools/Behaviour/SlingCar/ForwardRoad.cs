using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    public class ForwardRoad: MonoBehaviour
    {
        [SerializeField, Required] private Transform[] _LanePosReferences;

        private void OnDrawGizmos()
        {
            
            Gizmos.color = Color.yellow;
            foreach (var lanePosReference in _LanePosReferences)
            {
                Gizmos.DrawRay(new Vector3(lanePosReference.position.x, 0, 0), Vector3.forward * 100);
                
            }
        }

        public float GetPosForLane(int index)
        {
            return _LanePosReferences[index].position.x;
        }

        public int GetNextRoadIndex(int index)
        {
            index++;
            if (index > _LanePosReferences.Length - 1)
            {
                index = 0;
            }

            return index;
        }
    }
}