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

        public void Remove(int height, int size)
        {
            _oreCount -= size;
            for (var i = height; i < height + size; i++)
            {
                _ores[i] = null;
            }
        }

        public bool ShouldFall()
        {
            for (var i = 0; i < _oreCount; i++)
            {
                if (_ores[i] == null)
                {
                    return true;
                }
            }

            return false;
        }

        public void FallOne()
        {
            var foundHole = false;
            for (var i = 0; i < ColumnSize; i++)
            {
                if (!foundHole)
                {
                    if (_ores[i] == null)
                    {
                        foundHole = true;
                    }
                }
                else
                {
                    _ores[i - 1] = _ores[i];
                }
            }
        }
    }
}