namespace GameStructure
{
    public  class TimeResponder
    {
        protected TimeResponder()
        {
            GameStructureSystem.AddResponder(this);
        }

        public bool IsActive;
        
        public virtual void Start() {}

        public virtual void Update() {}

        public virtual void FixedUpdate() {}
    }
}