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
            for (var i = go.transform.childCount-1; i >= 0; i--)
            {
                Object.Destroy(go.transform.GetChild(i).gameObject);
            }
        }
    }
}