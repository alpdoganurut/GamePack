using UnityEngine;

namespace GameStructure
{
    public  class TimeResponder
    {
        protected TimeResponder()
        {
            GameStructureSystem.AddResponder(this);
        }

        public void Deconstruct()
        {
            Debug.Log("Destroyed");
        }

        public bool IsActive;
        
        public virtual void Start() {}

        public virtual void Update() {}

        public virtual void FixedUpdate() {}

        public void Destroy()
        {
            GameStructureSystem.RemoveResponder(this);
        }
    }
}