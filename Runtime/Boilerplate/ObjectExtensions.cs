using GamePack.Boilerplate.Structure;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GamePack.Boilerplate
{
    // ReSharper disable once UnusedType.Global
    public static class ObjectExtensions
    {
        // public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object => (T) Object.Instantiate((Object) original, position, rotation);
        public static T InstantiateInLevel<T>(this T obj, Vector3? position = null, Quaternion? rotation = null) where T : Object
        {
            if(SceneLevelManager.LoadedScene == null)
            {
                Debug.LogError($"Game.LoadedScene is null, not instantiating {obj.name}!", obj);
                return null;
            }

            var newObj = StructureMonoBehaviourBase.Instantiate(obj, position ?? Vector3.zero, rotation ?? Quaternion.identity);
            
            var go = GetGameObject(newObj);

            SceneManager.MoveGameObjectToScene(go, SceneLevelManager.LoadedScene.Value);

            return newObj;
        }

        public static T MoveToLevelScene<T>(this T obj) where T : Object
        {
            if(SceneLevelManager.LoadedScene == null)
            {
                Debug.LogError($"Game.LoadedScene is null, not instantiating {obj.name}!", obj);
                return null;
            }
            
            var go = GetGameObject(obj);
            SceneManager.MoveGameObjectToScene(go, SceneLevelManager.LoadedScene.Value);

            return obj;
        }

        private static GameObject GetGameObject<T>(T newObj) where T : Object
        {
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

            return go;
        }
    }
}