using System.IO;
using System.Linq;
using UnityEngine;

namespace Systems.Levels
{
    public enum LevelType
    {
        Harvester,
        ApplePicker,
    }
    
    [CreateAssetMenu(menuName = "Level", fileName = "level_")]
    public class LevelSo : ScriptableObject
    {
        public int LevelNumber => int.Parse(name.Split('_').Last());
        public LevelType levelType;
        public Texture levelFile;
        
        public int aGradeCount;
        public int bGradeCount;
        public int cGradeCount;
        
        public Texture LoadImage()
        {
            if (levelFile) return levelFile;
            var file = Resources.Load<Texture>($"Levels/{name}");
            if (file)
            {
                return file;
            }

            throw new FileLoadException();
        }
    }
    
    
}