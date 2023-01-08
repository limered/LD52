using SystemBase.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.UI.Infos
{
    public class UIInfoComponent : GameComponent
    {
        public Image vehicleImage;
        public Image harvestItemImage;
        public TextMeshProUGUI levelName;
        public Image grade;
        public Sprite[] vehicleSprites;
        public Sprite[] harvestSprites;
        public Sprite[] gradeSprites;
    }
}