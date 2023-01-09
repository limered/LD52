using System;
using System.Linq;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.Drescher;
using Systems.GameState;
using Systems.Grid;
using Systems.Levels;
using Systems.Levels.Events;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Systems.Critters
{
    public enum CritterType
    {
        Cow,
        Sheep
    }

    [GameSystem]
    public class CritterSystem : GameSystem<CritterInitComponent, CritterComponent, MainGridComponent>
    {
        private MainGridComponent _grid;

        public override void Register(CritterInitComponent initComponent)
        {
            MessageBroker.Default.Receive<SpawnPlayerMessage>()
                .Subscribe(_ => SpawnCritters(initComponent))
                .AddTo(initComponent);

            MessageBroker.Default.Receive<AskToGoToNextLevelMsg>()
                .Subscribe(_ => { initComponent.gameObject.RemoveAllChildren(); })
                .AddTo(initComponent);

            MessageBroker.Default.Receive<ShowLevelOverviewMsg>()
                .Subscribe(_ => { initComponent.gameObject.RemoveAllChildren(); })
                .AddTo(initComponent);
        }

        private void SpawnCritters(CritterInitComponent initComponent)
        {
            initComponent.gameObject.RemoveAllChildren();

            var type = IoC.Game.GetComponent<CurrentLevelComponent>().Level.LevelIndex % 2;
            var potentialPositions = _grid.backgroundGrid.GetCoordinatesOfType(BackgroundCellType.Path);
            var c = (int)(potentialPositions.Length * 0.4f);
            c = math.min(c, initComponent.critterCount);
            potentialPositions = potentialPositions.Randomize().Take(c).ToArray();
            if (!potentialPositions.Any()) return;

            foreach (var potentialPosition in potentialPositions)
                SpawnCritters(type == 0 ? initComponent.cowPrefab : initComponent.sheepPrefab, initComponent,
                    potentialPosition);
        }

        private void SpawnCritters(
            GameObject prefab,
            CritterInitComponent critterInitComponent,
            Vector2Int potentialPositions)
        {
            var critter = Object.Instantiate(
                prefab,
                new Vector3(potentialPositions.x, 0.2f, potentialPositions.y),
                Quaternion.identity,
                critterInitComponent.transform);

            SetNewMovementDirection(critter.GetComponent<CritterComponent>());
        }

        public override void Register(CritterComponent component)
        {
            component.startPosition = component.transform.position;
            component.cachedBody = component.GetComponent<Rigidbody>();
            component.cachedRenderer = component.GetComponent<Renderer>();
            SystemUpdate(component).Subscribe(MoveCritter).AddTo(component);
            Observable.Interval(TimeSpan.FromMilliseconds(Random.Range(500, 1000)))
                .Subscribe(_ => SetNewMovementDirection(component)).AddTo(component);
        }

        private void SetNewMovementDirection(CritterComponent critter)
        {
            var pos = critter.transform.position;
            var rndDirection = Random.insideUnitCircle;
            var directionToCenter = critter.startPosition - pos;

            critter.movementDirection.Value = math.normalize(math.lerp(
                math.normalize(math.float2(rndDirection.x, rndDirection.y)),
                math.normalize(math.float2(directionToCenter.x, directionToCenter.z)),
                math.clamp(directionToCenter.magnitude / critter.maxDistance, 0f, 1f)));
        }

        private static void MoveCritter(CritterComponent critter)
        {
            var nextStep = critter.movementDirection.Value * critter.speed * Time.deltaTime;
            critter.cachedBody.AddForce(new Vector3(nextStep.x, 0, nextStep.y));

            var sinus = math.sin(Time.realtimeSinceStartup * 5) * 0.002f - 0.001f;
            sinus += 0.1f;
            critter.transform.localScale = new Vector3(sinus, sinus, sinus);

            var vel = critter.cachedBody.velocity;
            if (math.abs(vel.x) > math.abs(vel.z))
            {
                if (vel.x < 0)
                    critter.cachedRenderer.material.mainTexture = critter.images[1];
                else
                    critter.cachedRenderer.material.mainTexture = critter.images[3];
            }
            else
            {
                if (vel.z < 0)
                    critter.cachedRenderer.material.mainTexture = critter.images[2];
                else
                    critter.cachedRenderer.material.mainTexture = critter.images[0];
            }
        }

        public override void Register(MainGridComponent component)
        {
            _grid = component;
        }
    }
}