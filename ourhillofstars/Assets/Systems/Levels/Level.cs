using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Systems.Levels
{
    [Serializable]
    public enum LevelType
    {
        Harvester,
        ApplePicker,
    }
    
    public class Level
    {
        public string levelFile;
        public LevelType levelType;
        public int LevelNumber => int.Parse(levelFile.Split('_').Last());
        public Sprite levelSprite;

        public DrescherDirection startDirection;
        public int aGradeCount;
        public int bGradeCount;
        public int cGradeCount;

        public string themeFile;
        public string playerThemeFile;
        
        public Sprite LoadImage()
        {
            if (levelSprite) return levelSprite;
            var file = Resources.Load<Sprite>($"Levels/{levelFile}");
            if (file)
            {
                return file;
            }

            throw new FileLoadException($"Levels/{levelFile}");
        }
    }
    
    
}