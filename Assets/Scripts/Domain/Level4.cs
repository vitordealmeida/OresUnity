namespace Domain.Entities
{
    public class Level4 : Level
    {
        private readonly float _baseTimeForColumn;

        public float TimeForColumnAdd => _baseTimeForColumn * .5f;
        public float ScoreMultiplier => 2.5f;
        public int OreTypeCount => 5;
        public int ScoreLimit => 100000000;

        public Level4(float baseTimeForColumn)
        {
            _baseTimeForColumn = baseTimeForColumn;
        }
    }
}