using SystemBase.Core;
using Systems.Grid;
using TMPro;
using UnityEngine.UI;

namespace Systems.UI
{
    public class ArrowElementComponent : GameComponent
    {
        public ForegroundCellType foregroundCellType;
        public TextMeshProUGUI amount;
        public Image arrow;
    }
}