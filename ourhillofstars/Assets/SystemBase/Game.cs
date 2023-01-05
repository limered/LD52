using SystemBase.Adapter;
using SystemBase.CommonSystems.Audio.Helper;
using SystemBase.Core;
using SystemBase.GameState.Messages;
using SystemBase.GameState.States;
using SystemBase.Utils;
using UniRx;
using UnityEngine;

namespace SystemBase
{
    public class Game : GameBase
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public StateContext<Game> gameStateContext;

        private void Awake()
        {
            gameStateContext = new StateContext<Game>(this);
            gameStateContext.Start(new Loading());

            Init();

            MessageBroker.Default.Publish(new GameMsgFinishedLoading());
            MessageBroker.Default.Publish(new GameMsgStart());
            Cursor.visible = true;
        }

        private void Start()
        {
            // MessageBroker.Default.Publish(new GameMsgStart());
            QualitySettings.vSyncCount = 0;
            // Application.targetFrameRate = 60;
        }

        public override void Init()
        {
            base.Init();

            IoC.RegisterSingleton(this, true);
            IoC.RegisterSingleton<ISFXComparer, SFXComparer>(true);
        }
    }
}