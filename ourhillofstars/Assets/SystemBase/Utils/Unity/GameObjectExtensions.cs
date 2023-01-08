using UnityEngine;

namespace SystemBase.Utils
{
    public static class GameObjectExtensions
    {
        public static bool TryGetComponent<TComp>(this GameObject go, out TComp component) where TComp : Component
        {
            component = go.GetComponent<TComp>();
            return component != null;
        }

        public static void RemoveAllChildren(this GameObject go)
        {
            go.transform.RemoveAllChildren();
        }
        
        public static void RemoveAllChildren(this Transform transform)
        {
            for (var i = transform.childCount-1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}