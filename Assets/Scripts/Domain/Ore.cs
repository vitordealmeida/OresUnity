namespace Domain.Entities
{
    public class Ore
    {
        public const int OreTypeCount = 6;

        public Ore(OreType oreType)
        {
            type = oreType;
        }

        public OreType type { get; }

        public override string ToString()
        {
            return type.ToString();
        }
    }
}