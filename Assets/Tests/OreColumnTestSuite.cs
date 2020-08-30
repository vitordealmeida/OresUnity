using System.Collections.Generic;
using Domain.Entities;
using NUnit.Framework;

namespace Tests
{
    public class OreColumnTestSuite
    {
        [Test]
        public void GenerateRandomColumn_HasProperHeight()
        {
            var oreColumn = OreColumn.GenerateRandomColumn(1);

            Assert.AreEqual(oreColumn.Count, OreColumn.ColumnSize);
        }

        [Test]
        public void GenerateRandomColumn_RespectsOreTypes()
        {
            var oreTypes = 3;
            var oreColumn = OreColumn.GenerateRandomColumn(oreTypes);

            var oreTypesVerified = new List<OreType>();
            for (int i = 0; i < oreColumn.Count; i++)
            {
                var ore = oreColumn.Get(i);
                if (!oreTypesVerified.Contains(ore.type))
                {
                    oreTypesVerified.Add(ore.type);
                }
            }

            Assert.LessOrEqual(oreTypesVerified.Count, oreTypes);
        }

        [Test]
        public void RemoveOre_DecreasesCount()
        {
            var oreColumn = OreColumn.GenerateRandomColumn(1);
            var count = oreColumn.Count;

            oreColumn.Remove(5);

            Assert.AreEqual(count - 1, oreColumn.Count);
        }

        [Test]
        public void RemoveOre_GetRemovedOre_ReturnsNull()
        {
            var oreColumn = OreColumn.GenerateRandomColumn(1);

            oreColumn.Remove(5);

            Assert.IsNull(oreColumn.Get(5));
        }

        [Test]
        public void ApplyGravity_NoOreRemoved_NothingChanges()
        {
            var oreColumn = OreColumn.GenerateRandomColumn(1);
            var oresList = new List<Ore>();

            for (int i = 0; i < oreColumn.Count; i++)
            {
                oresList.Add(oreColumn.Get(i));
            }

            oreColumn.ApplyGravity();

            for (int i = 0; i < oreColumn.Count; i++)
            {
                Assert.AreEqual(oresList[i], oreColumn.Get(i));
            }
        }

        [Test]
        public void ApplyGravity_OneOreRemoved_GetRemovedOreReturnsSome_GetTopOreReturnsNull()
        {
            var oreColumn = OreColumn.GenerateRandomColumn(1);

            oreColumn.Remove(5);
            oreColumn.ApplyGravity();

            Assert.IsNotNull(oreColumn.Get(5));
            Assert.IsNull(oreColumn.Get(OreColumn.ColumnSize - 1));
        }
    }
}