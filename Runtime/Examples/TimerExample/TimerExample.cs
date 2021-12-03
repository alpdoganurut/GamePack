using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePack.Timer
{
    public class TimerExample: MonoBehaviour
    {
        [SerializeField, Required] private bool  _IsSkip;
        [SerializeField, Required] private GameObject _ObjectToMove;
        [SerializeField] private EasingFunction.Ease _MoveObjectEase;
        [SerializeField] private float _InitialDelay = 2;
        
        private void Start()
        {
            var objectStartPos = _ObjectToMove.transform.position;
            
            var newOp =
                new Operation(name + " Root Op", delay: _InitialDelay, duration: 1, 
                    action: () =>
                    {
                        Debug.Log(name + " Root Op action");
                    })
                .Add(name + " Move object", delay: 2f,  duration: 3, ease: _MoveObjectEase, 
                    updateAction: (tVal) =>
                    {
                        _ObjectToMove.transform.position = Vector3.Lerp(objectStartPos, objectStartPos + new Vector3(0, 0, 5), tVal);
                    })
                .Add(name + " Second Op", duration: 2,
                    skipCondition: () => _IsSkip,
                    updateAction: (tVal) =>
                    {
                        Debug.Log(name + " Second Op update");
                        Debug.Log($"{name} tVal: {tVal}");
                    })
                .Add(new Operation(name + " Third Op", delay: 1,
                    action: () =>
                    {
                        Debug.Log(name + " Third Op action");
                        Debug.Log(name + " Waiting for space...");
                    },
                    finishCondition: () => Input.GetKeyDown(KeyCode.Space)))
                .Add(new Operation(name + " Fourth Op - Waiting for space to start", delay: 2, duration: 4, 
                    waitForCondition: () => Input.GetKeyDown(KeyCode.Space),
                    action: () =>
                    {
                        Debug.Log(name + " Moving object back");
                    }, 
                    updateAction: (tVal) =>
                    {
                        _ObjectToMove.transform.position = Vector3.Lerp(objectStartPos, objectStartPos + new Vector3(0, 0, 5), 1 - tVal);
                    }))
                .Save();

            newOp.Start();
        }
    }
}