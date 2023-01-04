using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR || UNITY_EDITOR_WIN
#endif

namespace SystemBase.Utils
{
#if UNITY_EDITOR || UNITY_EDITOR_WIN

    public class InspectorExtensions : MonoBehaviour
    {
        [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
        public class EnumFlagsAttributeDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
            }
        }
    }

#endif

    public class EnumFlagsAttribute : PropertyAttribute
    {
    }
}