using HexGames;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeesawCatapult
{
    public static class ObjectExtensions
    {
        // public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object => (T) Object.Instantiate((Object) original, position, rotation);
        public static T InstantiateInLevel<T>(this T obj, Vector3? position = null, Quaternion? rotation = null) where T : Object
        {
            if(GameBase.LoadedScene == null)
            {
                Debug.LogError($"Game.LoadedScene is null, not instantiating {obj.name}!", obj);
                return null;
            }

            var newObj = Object.Instantiate(obj, position ?? Vector3.zero, rotation ?? Quaternion.identity);
            
            GameObject go = null;
            switch (newObj)
            {
                case Component component:
                    go = component.gameObject;
                    break;
                case GameObject gameObject:
                    go = gameObject;
                    break;
                default:
                    Debug.LogError("Only instantiate Component or GameObjects!");
                    break;
            }
            
            SceneManager.MoveGameObjectToScene(go, GameBase.LoadedScene.Value);

            return newObj;
        }
    }
}