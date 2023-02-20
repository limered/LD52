using SystemBase.Core;
using UniRx;
using UnityEngine;

namespace Systems.Tutorial
{
    public class TutorialComponent : GameComponent
    {
        public ReactiveProperty<TutorialStep> currentStep = new();
        
        public GameObject messageAddArrow;
        public GameObject messageRotateArrow;
        // public GameObject messageRemoveArrow;
        public GameObject messageClickStart;
        public float waitTime = 1;
    }
}