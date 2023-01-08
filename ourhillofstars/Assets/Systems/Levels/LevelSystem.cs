using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Grid;
using Systems.Levels.Events;
using Systems.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Systems.Levels
{
    [GameSystem(typeof(PauseMenuSystem))]
    public class LevelSystem : GameSystem<LevelOverviewComponent>
    {
        public const string FurthestLevelKey = "furthest_level";

        public static string LevelGradeKey(int levelNumber)
        {
            return $"level_grade_{levelNumber}";
        }

        public override void Register(LevelOverviewComponent component)
        {
            var currentLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
            currentLevel.IsPaused.Value = true;
            currentLevel.GameState = GameState.GameState.LevelSelect;
            var levels = IoC.Resolve<IGetAllLevelsAndGrades>().GetAllLevelsWithGrade();
            ReloadLevelOverview(levels, component);
            HandleMessages(levels, component);
        }


        private void ReloadLevelOverview((Level level, Grade grade)[] levels, LevelOverviewComponent component)
        {
            var cellPrefab = IoC.Game.PrefabByName("LevelButton");

            var parentTransform = component.GetComponent<RectTransform>();
            var layoutGroup = component.GetComponent<GridLayoutGroup>();
            var biggerSize =
                Math.Max((parentTransform.rect.size.x - component.gridDimensions.x * layoutGroup.spacing.x) /
                         component.gridDimensions.x,
                    (parentTransform.rect.size.y - component.gridDimensions.y * layoutGroup.spacing.y) /
                    component.gridDimensions.y);

            var cellSize = new Vector2(biggerSize, biggerSize);
            layoutGroup.cellSize = cellSize;

            parentTransform.RemoveAllChildren();

            var furthestLevel = PlayerPrefs.GetInt(FurthestLevelKey, 0);

            var max = Math.Min(levels.Length, component.gridDimensions.x * component.gridDimensions.y);
            for (var i = 0; i < max; i++)
            {
                var cell = Object.Instantiate(cellPrefab,
                    Vector3.zero, Quaternion.Euler(0, 0, 0),
                    parentTransform);

                cell.GetComponentInChildren<TextMeshProUGUI>().text = $"#{levels[i].level.LevelIndex + 1}";
#if !DEBUG
                if (i <= furthestLevel)
#else
                if (true)
#endif
                {
                    cell.GetComponentInChildren<LevelCellComponent>().level = i;
                    cell.GetComponentInChildren<Button>().image.sprite = levels[i].level.LoadImage();
                }
                else
                {
                    cell.GetComponentInChildren<LevelCellComponent>().level = null;
                    cell.GetComponentInChildren<Button>().image.sprite = null;
                    cell.GetComponentInChildren<TextMeshProUGUI>().color = Color.grey;
                }
            }
        }

        private void HandleMessages((Level level, Grade grade)[] levels, LevelOverviewComponent component)
        {
            MessageBroker.Default.Receive<LoadLevelMsg>().Subscribe(msg =>
                {
                    var currentLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
                    component.transform.parent.gameObject.SetActive(false);
                    currentLevel.IsPaused.Value = false;
                    currentLevel.GameState = GameState.GameState.Playing;
                    MessageBroker.Default.Publish(new GridLoadMsg
                    {
                        Level = levels[msg.LevelIndex].level
                    });
                })
                .AddTo(component);

            MessageBroker.Default.Receive<ShowLevelOverviewMsg>()
                .Subscribe(_ =>
                {
                    var currentLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
                    currentLevel.IsPaused.Value = true;
                    currentLevel.GameState = GameState.GameState.LevelSelect;
                    component.gameObject.transform.parent.gameObject.SetActive(true);
                    ReloadLevelOverview(levels, component);
                });

            MessageBroker.Default.Receive<GoToNextLevelMsg>().Subscribe(msg =>
            {
                // did complete a new level?
                var furthestLevel = PlayerPrefs.GetInt(FurthestLevelKey, 0);
                var nextLevel = msg.CompletedLevel + 1;
                if (furthestLevel < nextLevel)
                {
                    PlayerPrefs.SetInt(FurthestLevelKey, nextLevel);
                }

                //did the grade improve?
                var lastGradeForCompletedLevel =
                    (Grade)PlayerPrefs.GetInt(LevelGradeKey(msg.CompletedLevel), (int)Grade.None);
                if (msg.Grade < lastGradeForCompletedLevel)
                {
                    PlayerPrefs.SetInt(LevelGradeKey(msg.CompletedLevel), (int)msg.Grade);
                }

                // start next level
                if (nextLevel < levels.Length)
                {
                    MessageBroker.Default.Publish(new LoadLevelMsg { LevelIndex = nextLevel });
                }
                else
                {
                    Debug.Log("Du hast das Spiel durchgespielt");
                    MessageBroker.Default.Publish(new FinishLastLevelMsg());
                }
            });
        }

        public class LevelComparer : IEqualityComparer<Level>
        {
            public bool Equals(Level x, Level y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.LevelIndex == y.LevelIndex;
            }

            public int GetHashCode(Level obj)
            {
                return obj.LevelIndex;
            }
        }

        public interface IGetAllLevelsAndGrades
        {
            (Level level, Grade grade)[] GetAllLevelsWithGrade();
        }

        public class GetAllLevelsAndGrades : IGetAllLevelsAndGrades
        {
            public (Level level, Grade grade)[] GetAllLevelsWithGrade()
            {
                Debug.Log("get levels");
                var allLevelJsons = Resources.LoadAll<TextAsset>("");
                var allLevels = allLevelJsons
                    .Where(x => x.name.StartsWith("level_"))
                    .Select((x) =>
                    {
                        var level = JsonConvert.DeserializeObject<Level>(x.text);
                        level.levelFile ??= x.name;
                        return level;
                    })
                    .OrderBy(so => so.LevelIndex)
                    .ToList();

                Debug.Log($"level count {allLevels.Count}");
                Debug.Assert(allLevels.Count() == allLevels.Distinct(new LevelComparer()).Count(),
                    "you have duplicate levels!");

                return allLevels
                    .Select(level =>
                        (level, (Grade)PlayerPrefs.GetInt(LevelGradeKey(level.LevelIndex), (int)Grade.None)))
                    .ToArray();
            }
        }

        public class LevelAdapterRegistrations : IIocRegistration
        {
            public void Register()
            {
                IoC.RegisterType<IGetAllLevelsAndGrades, GetAllLevelsAndGrades>();
            }
        }
    }
}