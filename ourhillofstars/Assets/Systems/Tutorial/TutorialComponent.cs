using SystemBase.Core;
using UniRx;
using UnityEngine;

namespace Systems.Tutorial
{
    public class TutorialComponent : GameComponent
    {
        public ReactiveProperty<TutorialStep> currentStep = new(TutorialStep.None);
        
        public GameObject messageAddArrow;
        public GameObject messageRotateArrow;
        public GameObject messageRemoveArrow;
        public GameObject messageClickStart;
    }
}