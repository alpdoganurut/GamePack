namespace GamePack.PoolingSystem
{
    public class PoolableGameObject: PoolableBase
    {
        internal override void OnStart()
        {
            gameObject.SetActive(true);
        }

        internal override void OnStop()
        {
            gameObject.SetActive(false);
        }
    }
}