using System;
using Domain.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public class MatchController : MonoBehaviour
    {
        private Game _game;
        public Transform spawnPoint;
        public OrePrefabMap[] orePrefabMaps;

        private Vector3 prefabSize;
        
        private void Awake()
        {
            prefabSize = orePrefabMaps[0].prefab.GetComponent<SpriteRenderer>().bounds.size;
            
            _game = new Game();
            
            Debug.Log("Initial game: ");
            Debug.Log(_game.ToString());

            var oreColumns = _game.Board.OreColumns;
            for (var i = 0; i < oreColumns.Count; i++)
            {
                var oreColumn = oreColumns[i];

                var columnHolder = InstantiateColumn(oreColumn);
                columnHolder.transform.localPosition = new Vector3(i * prefabSize.x, 0, 0);
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
                oreInstance.transform.localPosition = new Vector3(0, j * prefabSize.y, 0);
            }

            return columnHolder;
        }

        public void AddColumn()
        {
            _game.Board.AddColumn();
            
            Debug.Log("Added column: ");
            Debug.Log(_game.ToString());

            var oreColumn = _game.Board.OreColumns[_game.Board.OreColumns.Count - 1];
            var columnHolder = InstantiateColumn(oreColumn);
            columnHolder.transform.localPosition = new Vector3(-1 * prefabSize.x, 0, 0);
        }
    }
}