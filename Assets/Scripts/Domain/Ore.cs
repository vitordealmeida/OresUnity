namespace Domain.Entities
{
    public class Ore
    {
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