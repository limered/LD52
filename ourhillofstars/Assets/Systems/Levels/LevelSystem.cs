using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Grid;
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

        public string LevelGradeKey(int levelNumber)
        {
            return $"level_grade_{levelNumber}";
        }

        public override void Register(LevelOverviewComponent component)
        {
            var currentLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
            currentLevel.IsPaused = true;
            currentLevel.GameState = GameState.GameState.LevelSelect;
            var levels = GetLevels();
            ReloadLevelOverview(levels, component);
            HandleMessages(levels, component);

            // Example Load of Images
            var allLevels = Resources.LoadAll<LevelSo>($"").OrderBy(so => so.LevelNumber).ToArray();
            Debug.Assert(allLevels.Count() == allLevels.Distinct(new LevelSoComparer()).Count(),
                "you have duplicate levels!");
        }

        private void ReloadLevelOverview(List<LevelSo> levels, LevelOverviewComponent component)
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

            var max = Math.Min(levels.Count, component.gridDimensions.x * component.gridDimensions.y);
            for (var i = 0; i < max; i++)
            {
                var cell = Object.Instantiate(cellPrefab,
                    Vector3.zero, Quaternion.Euler(0, 0, 0),
                    parentTransform);

                cell.GetComponentInChildren<TextMeshProUGUI>().text = levels[i].name;
#if !DEBUG
                if (i <= furthestLevel)
#else
                if (true)
#endif
                {
                    cell.GetComponentInChildren<LevelCellComponent>().level = i;
                    cell.GetComponentInChildren<Button>().image.sprite = levels[i].LoadImage();
                }
                else
                {
                    cell.GetComponentInChildren<LevelCellComponent>().level = null;
                    cell.GetComponentInChildren<Button>().image.sprite = null;
                    cell.GetComponentInChildren<TextMeshProUGUI>().color = Color.grey;
                }
            }
        }

        private void HandleMessages(List<LevelSo> levels, LevelOverviewComponent component)
        {
            MessageBroker.Default.Receive<LoadLevelMsg>().Subscribe(msg =>
                {
                    var currentLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
                    component.transform.parent.gameObject.SetActive(false);
                    currentLevel.IsPaused = false;
                    currentLevel.GameState = GameState.GameState.Playing;
                    MessageBroker.Default.Publish(new GridLoadMsg
                    {
                        Level = levels[msg.LevelIndex]
                    });
                })
                .AddTo(component);

            MessageBroker.Default.Receive<ShowLevelOverviewMsg>()
                .Subscribe(_ =>
                {
                    var currentLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
                    currentLevel.IsPaused = true;
                    currentLevel.GameState = GameState.GameState.LevelSelect;
                    component.gameObject.transform.parent.gameObject.SetActive(true);
                    ReloadLevelOverview(levels, component);
                });

            MessageBroker.Default.Receive<LevelCompleteMsg>().Subscribe(msg =>
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
                if (nextLevel < levels.Count)
                {
                    MessageBroker.Default.Publish(new LoadLevelMsg { LevelIndex = nextLevel });
                }
                else
                {
                    Debug.Log("Du hast das Spiel durchgespielt");
                }
            });
        }

        private List<LevelSo> GetLevels()
        {
            var allLevels = Resources.LoadAll<LevelSo>($"").OrderBy(so => so.LevelNumber).ToList();
            Debug.Assert(allLevels.Count() == allLevels.Distinct(new LevelSoComparer()).Count(),
                "you have duplicate levels!");
            return allLevels;
        }

        public class LevelSoComparer : IEqualityComparer<LevelSo>
        {
            public bool Equals(LevelSo x, LevelSo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.LevelNumber == y.LevelNumber;
            }

            public int GetHashCode(LevelSo obj)
            {
                return obj.LevelNumber;
            }
        }
    }
}