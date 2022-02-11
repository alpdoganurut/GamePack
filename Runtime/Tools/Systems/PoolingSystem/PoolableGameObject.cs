namespace GamePack.Poolable
{
    public class PoolableGameObject: PoolableBase
    {
        public override void OnStart()
        {
            gameObject.SetActive(true);
        }

        public override void OnStop()
        {
            gameObject.SetActive(false);
        }
    }
}