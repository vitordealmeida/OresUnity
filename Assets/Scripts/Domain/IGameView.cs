using System.Collections.Generic;
using Unity.Mathematics;

namespace Domain.Entities
{
    public interface IGameView
    {
        void AddNewColumn(OreColumn oreColumn);
        void ClearOresAt(List<int2> oreCluster, int clusterScore);
        void ApplyGravity();
        void ShowGameOver();
        void RemoveColumns(List<int> emptyColumns);
        void UpdateScore(int score);
        void ScheduleColumnAdd();
    }
}