using System.Collections;
using ExampleSystems.Example.States;
using SystemBase.Core;
using SystemBase.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ExampleSystems.Example
{
    [GameSystem]
    public class FunnyMovementSystem : GameSystem<FunnyMovementComponent, FunnyMovementConfigComponent>
    {
        private float _speed;
        private FunnyMovementConfigComponent _config;

        public override void Init()
        {
            //could be added to IoC
            IoC.RegisterSingleton(this);
        }

        public override void Register(FunnyMovementComponent currentChunk)
        {
            if (!currentChunk) return;

            //UniRX Magic
            currentChunk.UpdateAsObservable()
                .Where(_ => _config != null)
                .Where((_, i) => i % 60 == 0)
                .Where(_ => _config.MovementState.CurrentState.Value.GetType() == typeof(Moving))

                /*
                 * This Logging extensions can be put anywhere in the observable-creation-call-chain
                 * to intercept values/errors and print them before they reach your Subscribtion methods.
                 */
                .LogError() // Logs OnError and prints exception by using Debug.LogException(). Optionally you can provide a format function.
                .LogOnNext("move funny") //Logs every OnNext value. Optionally you can provide a format string where {0} is replaced by the value.

                .Subscribe(_ => MoveFunny(currentChunk))
                .AddTo(currentChunk);
        }

        public override void Register(FunnyMovementConfigComponent comp)
        {
            if (!comp) return;

            _config = comp;

            comp.Speed
                .Subscribe(speed => _speed = speed)
                .AddTo(comp);

            comp.MovementState.Start(new Standing());
        }

        private void MoveFunny(FunnyMovementComponent comp)
        {
            var direction = new Vector3().RandomVector(new Vector3(-_speed, -_speed, -_speed), new Vector3(_speed, _speed, _speed));

            //CoRoutines are normally only possible on MonoBehaviour/Render Thread. This is again UniRx Magic
            MainThreadDispatcher.StartUpdateMicroCoroutine(MoveStraight(direction, comp));
        }

        /**
         * CoRoutine
         */

        private IEnumerator MoveStraight(Vector3 direction, Component comp)
        {
            for (var i = 0; i < 30; i++)
            {
                comp.GetComponent<Rigidbody>().AddForce(direction);
                yield return null;
            }
        }
    }
}