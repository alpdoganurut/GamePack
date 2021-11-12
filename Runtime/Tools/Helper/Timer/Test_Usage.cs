using UnityEngine;

namespace GamePack.Timer
{
    public class Test_Usage: MonoBehaviour
    {
        private void Start()
        {
            var newOp =
                new Operation("Root Op", delay: 1, duration: 1, 
                    action: () =>
                    {
                        Debug.Log("Root Op action");
                    })
                .Add("Second Op", duration: 2,
                    // skipCondition: () => true,
                    updateAction: () =>
                    {
                        Debug.Log("Second Op update");
                    })
                .Add(new Operation("Third Op", delay: 1,
                    action: () =>
                    {
                        Debug.Log("Third Op action");
                        Debug.Log("Waiting for space...");
                    },
                    finishCondition: () => Input.GetKeyDown(KeyCode.Space)))
                .Add(new Operation("Fourth Op", delay: 2,
                    waitForCondition: () =>
                    {
                        Debug.Log("Fourth Op waitForCondition");
                        return Input.GetKeyDown(KeyCode.Space);
                    },
                    action: () =>
                    {
                        Debug.Log("Fourth Op action");
                    }))
                    // finishCondition: () => Input.GetKeyDown(KeyCode.Space)))
                .Save();

            newOp.Start();
        }
    }
}