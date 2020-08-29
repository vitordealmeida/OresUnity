using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Domain.Entities
{
    public class Game
    {
        private readonly IGameView _view;
        private const int InitialOreTypes = 4;
        public const int MaxColumns = 16;
        private readonly Board _board;

        private readonly List<Level> _levels;

        private bool _gameOver;
        private int _score;
        private Level _currentLevel;

        public Game(MatchController view, float initialTimeForColumn)
        {
            _view = view;
            _board = new Board(InitialOreTypes);
            _levels = new List<Level>
            {
                new Level1(initialTimeForColumn),
                new Level2(initialTimeForColumn),
                new Level3(initialTimeForColumn),
                new Level4(initialTimeForColumn)
            };
            
            _currentLevel = _levels[0];
            
            _view.UpdateScore(0);
            _view.ScheduleColumnAdd(_currentLevel.TimeForColumnAdd);
        }

        public override string ToString()
        {
            return _board.ToString();
        }

        public void OnOreClicked(Ore ore)
        {
            if (_gameOver) return;

            var oreCluster = _board.FindOreClusterCoordinates(ore);
            if (oreCluster.Count <= 1)
            {
                return;
            }

            _board.ClearOresAt(oreCluster);
            var clusterScore = (int) Math.Pow(oreCluster.Count, 2);
            IncreaseScore(clusterScore);

            _view.ClearOresAt(oreCluster, clusterScore);

            var emptyColumns = _board.FindEmptyColumns();

            _board.RemoveColumns(emptyColumns);
            _board.ApplyGravity();

            _view.RemoveColumns(emptyColumns);
            _view.ApplyGravity();
        }

        private void IncreaseScore(int clusterScore)
        {
            _score += (int) (clusterScore * _currentLevel.ScoreMultiplier);
            _view.UpdateScore(_score);
            if (_score > _currentLevel.ScoreLimit)
            {
                var curLevelIndex = _levels.FindIndex(level => level == _currentLevel);
                if (curLevelIndex == _levels.Count - 1)
                {
                    GameOver();
                }
                else
                {
                    _currentLevel = _levels[curLevelIndex + 1];
                    _view.DisplayLevelUp();
                }
            }
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
            if (_gameOver) return;
            _board.AddColumn(_currentLevel.OreTypeCount);
            _view.AddNewColumn(_board.OreColumns[0]);
            if (_board.OreColumns.Count > MaxColumns)
            {
                GameOver();
            }

            _view.ScheduleColumnAdd(_currentLevel.TimeForColumnAdd);
            Debug.Log(_board);
        }

        private void GameOver()
        {
            _gameOver = true;
            _view.ShowGameOver();
        }

        public List<OreColumn> GetOreColumns()
        {
            return _board.OreColumns;
        }
    }
}