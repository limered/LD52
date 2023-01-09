using SystemBase.Core;
using UnityEngine;

namespace Systems.Critters
{
    public class CritterInitComponent : GameComponent
    {
        public int critterCount;
        public GameObject cowPrefab;
        public GameObject sheepPrefab;
    }
}