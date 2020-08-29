using System;
using System.Collections;
using System.Collections.Generic;
using Domain.Entities;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MatchController : MonoBehaviour, IGameView
    {
        private Game _game;
        [Header("Game Basics")] public Transform spawnPoint;
        public OrePrefabMap[] orePrefabMaps;

        [Header("Game Visuals")] [Range(1, 5)] public float animationSpeed = 1;

        [Header("Game Parameters")] public float initialTimeForColumn;

        [Header("UI")] public Text gameScore;
        public Text lastMovementScore;
        public RectTransform lastMovementScoreHolder;
        public Image timeProgress;
        public Animator uiAnimator;
        public Canvas uiCanvas;

        private Vector3 _prefabSize;
        private const float ComparisonThreshold = .0001f;
        private static readonly int LevelUpTriggerHash = Animator.StringToHash("LevelUp");
        private static readonly int ScoreTriggerHash = Animator.StringToHash("Score");

        private void Awake()
        {
            _prefabSize = orePrefabMaps[0].prefab.GetComponent<SpriteRenderer>().bounds.size;

            _game = new Game(this, initialTimeForColumn);

            var oreColumns = _game.GetOreColumns();
            for (var i = 0; i < oreColumns.Count; i++)
            {
                var oreColumn = oreColumns[i];

                var columnHolder = InstantiateColumn(oreColumn);
                columnHolder.transform.localPosition = new Vector3(i * _prefabSize.x, 0, 0);
            }
        }

        private GameObject InstantiateColumn(OreColumn oreColumn)
        {
            var columnHolder = new GameObject("OreColumn");
            columnHolder.transform.SetParent(spawnPoint, false);
            for (int j = 0; j < oreColumn.Count; j++)
            {
                var ore = oreColumn.Get(j);
                var oreInstance =
                    Instantiate(Array.Find(orePrefabMaps, map => map.oreType == ore.type).prefab,
                        columnHolder.transform, false);
                oreInstance.GetComponent<OreController>().Init(ore, _game.OnOreClicked);
                oreInstance.transform.localPosition = new Vector3(0, j * _prefabSize.y, 0);
            }

            return columnHolder;
        }

        public void OnAddColumnClicked()
        {
            _game.OnAddColumnClicked();
        }

        [BurstCompile]
        private struct MoveColumnsLeftJob : IJobParallelForTransform
        {
            public float PrefabWidth;

            public void Execute(int index, TransformAccess transform)
            {
                transform.position += Vector3.left * PrefabWidth;
            }
        }

        [BurstCompile]
        private struct MoveColumnsRightJob : IJobParallelForTransform
        {
            public float StepSize;

            public void Execute(int index, TransformAccess transform)
            {
                transform.position += Vector3.right * StepSize;
            }
        }

        [BurstCompile]
        private struct MoveOresDownJob : IJobParallelForTransform
        {
            public float StepSize;

            public void Execute(int index, TransformAccess transform)
            {
                transform.position += Vector3.down * StepSize;
            }
        }

        public void AddNewColumn(OreColumn oreColumn)
        {
            var columnHolder = InstantiateColumn(oreColumn);
            columnHolder.transform.SetAsFirstSibling();
            columnHolder.transform.localPosition = new Vector3(-1 * _prefabSize.x, 0, 0);

            var transformAccessArray = new TransformAccessArray(_game.GetOreColumns().Count);

            for (int i = 0; i < spawnPoint.transform.childCount; i++)
            {
                transformAccessArray.Add(spawnPoint.transform.GetChild(i));
            }

            new MoveColumnsLeftJob
                {
                    PrefabWidth = _prefabSize.x
                }
                .Schedule(transformAccessArray)
                .Complete();

            transformAccessArray.Dispose();
        }

        public void ClearOresAt(List<int2> oreCluster, int clusterScore)
        {
            ShowEarnedPoints(clusterScore);

            // Sorts asc on column, desc on height 
            oreCluster.Sort((coord1, coord2) =>
            {
                var columnCompare = coord1.x.CompareTo(coord2.x);
                return columnCompare != 0 ? columnCompare : coord2.y.CompareTo(coord1.y);
            });

            foreach (var oreCoord in oreCluster)
            {
                var oreTransform = spawnPoint.GetChild(oreCoord.x).GetChild(oreCoord.y);
                oreTransform.gameObject.SetActive(false);
                oreTransform.SetParent(null);
                var oreDestructionEffect = oreTransform.GetComponent<OreController>().DestructionEffectPrefab();
                Instantiate(oreDestructionEffect, oreTransform.position, Quaternion.identity);
                Destroy(oreTransform.gameObject);
            }
        }

        private void ShowEarnedPoints(int clusterScore)
        {
            lastMovementScore.text = clusterScore.ToString();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvas.transform as RectTransform,
                Input.mousePosition, uiCanvas.worldCamera,
                out Vector2 mousePositionCanvas);
            lastMovementScoreHolder.transform.position = uiCanvas.transform.TransformPoint(mousePositionCanvas);
            uiAnimator.SetTrigger(ScoreTriggerHash);
        }

        public void ApplyGravity()
        {
            for (var i = 0; i < spawnPoint.childCount; i++)
            {
                StartCoroutine(GravityAnimationCoroutine(i));
            }
        }

        public void ShowGameOver()
        {
            Time.timeScale = 0;
            Debug.Log("Game over");
        }

        public void RemoveColumns(List<int> emptyColumns)
        {
            emptyColumns.Sort((c1, c2) => c2.CompareTo(c1));
            foreach (var emptyColumn in emptyColumns)
            {
                var columnTransform = spawnPoint.GetChild(emptyColumn);
                columnTransform.gameObject.SetActive(false);
                columnTransform.transform.SetParent(null);
                Destroy(columnTransform.gameObject);
            }

            StartCoroutine(ColumnFoldingCoroutine());
        }

        public void UpdateScore(int score)
        {
            gameScore.text = score.ToString();
        }

        public void DisplayLevelUp()
        {
            uiAnimator.SetTrigger(LevelUpTriggerHash);
        }

        private Coroutine _columnAddTimer;

        public void ScheduleColumnAdd(float timeForColumnAdd)
        {
            if (_columnAddTimer != null)
            {
                StopCoroutine(_columnAddTimer);
            }

            _columnAddTimer = StartCoroutine(TimerForColumnAddition(timeForColumnAdd));
        }

        private IEnumerator TimerForColumnAddition(float timeLimit)
        {
            var initialTime = Time.time;
            while (Time.time - initialTime < timeLimit)
            {
                timeProgress.fillAmount = 1 - (Time.time - initialTime) / timeLimit;
                yield return null;
            }

            _game.OnTimeExpiredColumn();
        }

        private IEnumerator ColumnFoldingCoroutine()
        {
            while (true)
            {
                var transformAccessArray = new TransformAccessArray(Game.MaxColumns);

                float gapDistance = 0;
                for (var i = 0; i < spawnPoint.childCount; i++)
                {
                    var columnTransform = spawnPoint.GetChild(i);
                    if (gapDistance < ComparisonThreshold)
                    {
                        gapDistance = columnTransform.localPosition.x - i * _prefabSize.x;
                        if (gapDistance > ComparisonThreshold)
                        {
                            transformAccessArray.Add(columnTransform);
                        }
                    }
                    else
                    {
                        transformAccessArray.Add(columnTransform);
                    }
                }

                if (gapDistance < ComparisonThreshold)
                {
                    transformAccessArray.Dispose();
                    break;
                }

                new MoveColumnsRightJob()
                    {
                        StepSize = Math.Min(Time.deltaTime * animationSpeed, gapDistance)
                    }
                    .Schedule(transformAccessArray)
                    .Complete();

                transformAccessArray.Dispose();
                yield return null;
            }
        }

        private IEnumerator GravityAnimationCoroutine(int column)
        {
            var columnTransform = spawnPoint.GetChild(column);
            while (true)
            {
                var transformAccessArray = new TransformAccessArray(OreColumn.ColumnSize);

                float gapDistance = 0;
                for (var i = 0; i < columnTransform.childCount; i++)
                {
                    var oreTransform = columnTransform.GetChild(i);
                    if (gapDistance < ComparisonThreshold)
                    {
                        gapDistance = oreTransform.localPosition.y - i * _prefabSize.y;
                        if (gapDistance > ComparisonThreshold)
                        {
                            transformAccessArray.Add(oreTransform);
                        }
                    }
                    else
                    {
                        transformAccessArray.Add(oreTransform);
                    }
                }

                if (gapDistance < ComparisonThreshold)
                {
                    transformAccessArray.Dispose();
                    break;
                }

                new MoveOresDownJob()
                    {
                        StepSize = Math.Min(Time.deltaTime * animationSpeed, gapDistance)
                    }
                    .Schedule(transformAccessArray)
                    .Complete();

                transformAccessArray.Dispose();
                yield return null;
            }
        }
    }
}