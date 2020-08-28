using System;
using Domain.Entities;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;

namespace DefaultNamespace
{
    public class MatchController : MonoBehaviour
    {
        private Game _game;
        public Transform spawnPoint;
        public OrePrefabMap[] orePrefabMaps;

        private Vector3 _prefabSize;

        private void Awake()
        {
            _prefabSize = orePrefabMaps[0].prefab.GetComponent<SpriteRenderer>().bounds.size;

            _game = new Game();

            var oreColumns = _game.Board.OreColumns;
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
                var oreInstance =
                    Instantiate(Array.Find(orePrefabMaps, map => map.oreType == oreColumn.Get(j).type).prefab,
                        columnHolder.transform, false);
                oreInstance.transform.localPosition = new Vector3(0, j * _prefabSize.y, 0);
            }

            return columnHolder;
        }

        public void AddColumn()
        {
            _game.Board.AddColumn();

            var createdOreColumn = _game.Board.OreColumns[_game.Board.OreColumns.Count - 1];
            var columnHolder = InstantiateColumn(createdOreColumn);
            columnHolder.transform.localPosition = new Vector3(-1 * _prefabSize.x, 0, 0);

            var transformAccessArray = new TransformAccessArray(_game.Board.OreColumns.Count);

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
        public struct MoveColumnsRightJob : IJobParallelForTransform
        {
            public float prefabWidth;

            public void Execute(int index, TransformAccess transform)
            {
                transform.position += Vector3.right * prefabWidth;
            }
        }

        [BurstCompile]
        public struct MoveOresDownJob : IJobParallelForTransform
        {
            public float prefabHeight;

            public void Execute(int index, TransformAccess transform)
            {
                transform.position += Vector3.down * prefabHeight;
            }
        }
    }
}