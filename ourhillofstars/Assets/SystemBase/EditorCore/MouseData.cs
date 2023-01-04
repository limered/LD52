using UnityEngine;

namespace SystemBase.EditorCore
{
    public struct MouseData
    {
        /// <summary>
        /// 2D Mouse Position only over editor render window (Center is (0,0))
        /// </summary>
        public Vector2 editorPosition;
        
        /// <summary>
        /// Mouse Position in World (z-coord is camera coord)
        /// </summary>
        public Vector3 mouseWorldPosition;
        
        /// <summary>
        /// Button Numbers: -1 no button, 0 left, 2 right, 3 middle
        /// </summary>
        public int mouseButtonClicked;
    }
}