using System.Collections.Generic;

namespace Domain.Entities
{
    public class Board
    {
        private const int InitialColumnCount = 7;
        private List<OreColumn> oreColumns;
        private int _oreTypes;

        public Board(int oreTypes)
        {
            _oreTypes = oreTypes;
            oreColumns = new List<OreColumn>(InitialColumnCount);
            for (var i = 0; i < InitialColumnCount; i++)
            {
                AddColumn();
            }
        }

        public void IncreaseOreTypes()
        {
            _oreTypes += 1;
        }

        public void AddColumn()
        {
            oreColumns.Add(OreColumn.GenerateRandomColumn(_oreTypes));
        }

        public List<Ore> FindOresForClear(int column, int height)
        {
            var visitedOres = new List<Ore>();
            var ore = oreColumns[column].Get(height);
            if (ore == null) return visitedOres;

            visitedOres.Add(ore);
            VisitAllNeighbours(column, height, visitedOres);

            return visitedOres;
        }

        private void VisitAllNeighbours(int column, int height, List<Ore> visitedOres)
        {
            VisitOre(column - 1, height - 1, visitedOres);
            VisitOre(column + 1, height - 1, visitedOres);
            VisitOre(column - 1, height + 1, visitedOres);
            VisitOre(column + 1, height + 1, visitedOres);
        }

        private void VisitOre(int column, int height, List<Ore> visitedOres)
        {
            if (column < 0 || column >= oreColumns.Count || height < 0 || height >= OreColumn.ColumnSize)
            {
                return;
            }

            var ore = oreColumns[column].Get(height);
            if (ore == null || visitedOres.Contains(ore) || ore.type != visitedOres[0].type)
            {
                return;
            }

            visitedOres.Add(ore);
            VisitAllNeighbours(column, height, visitedOres);
        }

        public override string ToString()
        {
            var debug = "";
            for (int i = OreColumn.ColumnSize - 1; i >= 0; i--)
            {
                debug += "\n[";
                foreach (var oreColumn in oreColumns)
                {
                    debug += oreColumn.Get(i) + ",";
                }

                debug += "]";
            }

            return debug;
        }
    }
}