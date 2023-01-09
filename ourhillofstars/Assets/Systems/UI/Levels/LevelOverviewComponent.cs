using SystemBase.Core;
using UnityEngine;

namespace Systems.UI.Levels
{
    public class LevelOverviewComponent : GameComponent
    {
        public Vector2Int gridDimensions = new Vector2Int(5, 5);
        public Sprite[] levelThumbnails;
        public Sprite[] gradeSprites;
    }
}