using System;
using UnityEngine;

namespace GameStructure
{
    public class GameStructureSceneHelper: MonoBehaviour
    {
        public event Action StartEvent;
        public event Action UpdateEvent;
        public event Action FixedUpdateEvent;
        
        /*
        private void Awake()
        {
            throw new NotImplementedException();
        }
        */

        private void Start()
        {
            StartEvent?.Invoke();
        }

        private void Update()
        {
            UpdateEvent?.Invoke();
        }
        private void FixedUpdate()
        {
            FixedUpdateEvent?.Invoke();
        }

        // TODO Might change
        /*private void OnDestroy()
        {
            throw new NotImplementedException();
        }*/
    }
}