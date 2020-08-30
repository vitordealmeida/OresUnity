using System;

namespace Domain.Entities
{
    public class OreColumn
    {
        public const int ColumnSize = 10;
        private readonly Ore[] _ores;
        private int _oreCount;

        private OreColumn()
        {
            _ores = new Ore[ColumnSize];
        }

        public int Count => _oreCount;

        private void Push(Ore ore)
        {
            _ores[_oreCount++] = ore;
        }

        private static readonly Random _random = new Random();
        public static OreColumn GenerateRandomColumn(int oreTypes)
        {
            var oreColumn = new OreColumn();
            for (var i = 0; i < ColumnSize; i++)
            {
                oreColumn.Push(new Ore((OreType) _random.Next(0, oreTypes)));
            }
            return oreColumn;
        }

        public Ore Get(int height)
        {
            return _ores[height];
        }

        public void Remove(int height)
        {
            _oreCount -= 1;
            _ores[height] = null;
        }

        private int FindGapIndex(int startPosition = 0)
        {
            for (var i = startPosition; i < _oreCount; i++)
            {
                if (_ores[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        private void FallOne(int startPosition)
        {
            for (var i = startPosition; i < ColumnSize - 1; i++)
            {
                _ores[i] = _ores[i + 1];
            }

            _ores[ColumnSize - 1] = null;
        }

        public void ApplyGravity()
        {
            var gapIndex = FindGapIndex();
            while (gapIndex > -1)
            {
                FallOne(gapIndex);
                gapIndex = FindGapIndex();
            }
        }
    }
}