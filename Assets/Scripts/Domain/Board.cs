using System.Collections.Generic;
using Unity.Mathematics;

namespace Domain.Entities
{
    public class Board
    {
        private const int InitialColumnCount = 7;

        public List<OreColumn> OreColumns { get; }

        public Board(int oreTypes)
        {
            OreColumns = new List<OreColumn>(InitialColumnCount);
            for (var i = 0; i < InitialColumnCount; i++)
            {
                OreColumns.Add(OreColumn.GenerateRandomColumn(oreTypes));
            }
        }

        public void AddColumn(int oreTypeCount)
        {
            OreColumns.Insert(0, OreColumn.GenerateRandomColumn(oreTypeCount));
        }

        private int2 FindOreCoords(Ore ore)
        {
            for (var i = 0; i < OreColumns.Count; i++)
            {
                var oreColumn = OreColumns[i];
                for (int j = 0; j < oreColumn.Count; j++)
                {
                    if (oreColumn.Get(j) == ore)
                    {
                        return new int2(i, j);
                    }
                }
            }

            return new int2(-1, -1);
        }

        public List<int2> FindOreClusterCoordinates(Ore ore)
        {
            var visitedOres = new List<int2>();
            if (ore == null) return visitedOres;
            var oreCoords = FindOreCoords(ore);

            visitedOres.Add(oreCoords);
            VisitAllNeighbours(oreCoords.x, oreCoords.y, ore.type, visitedOres);

            return visitedOres;
        }

        private void VisitAllNeighbours(int column, int height, OreType oreType, List<int2> visitedOresCoordinates)
        {
            VisitOre(column - 1, height, oreType, visitedOresCoordinates);
            VisitOre(column + 1, height, oreType, visitedOresCoordinates);
            VisitOre(column, height - 1, oreType, visitedOresCoordinates);
            VisitOre(column, height + 1, oreType, visitedOresCoordinates);
        }

        private void VisitOre(int column, int height, OreType oreType, List<int2> visitedOres)
        {
            if (column < 0 || column >= OreColumns.Count || height < 0 || height >= OreColumn.ColumnSize)
            {
                return;
            }

            var ore = OreColumns[column].Get(height);
            if (ore == null || ore.type != oreType ||
                visitedOres.Exists(coord => coord.x == column && coord.y == height))
            {
                return;
            }

            visitedOres.Add(new int2(column, height));
            VisitAllNeighbours(column, height, oreType, visitedOres);
        }

        public override string ToString()
        {
            var debug = "";
            for (int i = OreColumn.ColumnSize - 1; i >= 0; i--)
            {
                debug += "\n[";
                foreach (var oreColumn in OreColumns)
                {
                    var ore = oreColumn.Get(i);
                    debug += ore == null ? "-," : ore + ",";
                }

                debug += "]";
            }

            return debug;
        }

        public void ClearOresAt(List<int2> oreCluster)
        {
            foreach (var oreCoord in oreCluster)
            {
                OreColumns[oreCoord.x].Remove(oreCoord.y);
            }
        }
        
        public void ApplyGravity()
        {
            foreach (var oreColumn in OreColumns)
            {
                oreColumn.ApplyGravity();
            }
        }

        public List<int> FindEmptyColumns()
        {
            var emptyColumns = new List<int>();
            for (var i = 0; i < OreColumns.Count; i++)
            {
                var oreColumn = OreColumns[i];
                if (oreColumn.Count == 0)
                {
                    emptyColumns.Add(i);
                }
            }

            return emptyColumns;
        }

        public void RemoveColumns(List<int> emptyColumns)
        {
            emptyColumns.Sort((c1, c2) => c2.CompareTo(c1));
            foreach (var emptyColumn in emptyColumns)
            {
                OreColumns.RemoveAt(emptyColumn);
            }
        }
    }
}