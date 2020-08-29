using System;
using System.Collections.Generic;
using UnityEngine;

namespace Domain.Entities
{
    public class Game
    {
        private readonly IGameView _view;
        private const int InitialOreTypes = 4;
        public const int MaxColumns = 16;
        private readonly Board _board;

        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                _view.UpdateScore(value);
            }
        }

        private bool gameOver;
        private int _score;

        public Game(IGameView view)
        {
            _view = view;
            _board = new Board(InitialOreTypes);
            _view.UpdateScore(0);
            _view.ScheduleColumnAdd();
            Debug.Log("Board: ");
            Debug.Log(_board);
        }

        public override string ToString()
        {
            return _board.ToString();
        }

        public void OnOreClicked(Ore ore)
        {
            var oreCluster = _board.FindOreClusterCoordinates(ore);
            if (oreCluster.Count <= 1)
            {
                return;
            }

            _board.ClearOresAt(oreCluster);
            var clusterScore = (int) Math.Pow(oreCluster.Count, 2);
            Score += clusterScore;

            _view.ClearOresAt(oreCluster, clusterScore);

            var emptyColumns = _board.FindEmptyColumns();

            _board.RemoveColumns(emptyColumns);

            _board.ApplyGravity();

            _view.RemoveColumns(emptyColumns);
            _view.ApplyGravity();
        }

        public void OnTimeExpiredColumn()
        {
            AddColumn();
        }

        public void OnAddColumnClicked()
        {
            AddColumn();
        }

        private void AddColumn()
        {
            _board.AddColumn();
            _view.AddNewColumn(_board.OreColumns[0]);
            if (_board.OreColumns.Count > MaxColumns)
            {
                GameOver();
            }
            _view.ScheduleColumnAdd();
            Debug.Log(_board);
        }

        private void GameOver()
        {
            gameOver = true;
            _view.ShowGameOver();
        }

        public List<OreColumn> GetOreColumns()
        {
            return _board.OreColumns;
        }
    }
}