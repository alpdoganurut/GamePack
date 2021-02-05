using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack
{
    public abstract class SlingCarLaneController : SlingCarControllerBase
    {
        [SerializeField, Required] private int _Lane;

        public int Lane
        {
            get => _Lane;

            set
            {
                SlingCar.TargetSidePos = _Road.GetPosForLane(value);
                _Lane = value;
            }
        }
        
        protected virtual void Awake()
        {
            if (_Road)
                Lane = _Lane;
            // SlingCar.TargetSidePos = _Road.GetPos(Lane);

            // _SlingCar.gameObject.AddComponent<CarControllerDelegate>().Init(this);
        }

        public override void SetRoad(ForwardRoad road)
        {
            base.SetRoad(road);
            Lane = _Lane;
        }
    }
    
    /*internal class CarControllerDelegate: MonoBehaviour
    {
        public SlingCarController CarController { get; private set; }
        public CarControllerDelegate Init(SlingCarController playerCar)
        {
            CarController = playerCar;
            return this;
        }
    }

    internal class PlayerCarControllerDelegate : CarControllerDelegate
    {
        public PlayerCar PlayerCarController => CarController as PlayerCar;
    }*/
}