namespace Domain.Entities
{
    public class Level2 : Level
    {
        private readonly float _baseTimeForColumn;

        public float TimeForColumnAdd => _baseTimeForColumn * .75f;
        public float ScoreMultiplier => 1.5f;
        public int OreTypeCount => 4;
        public int ScoreLimit => 1000;

        public Level2(float baseTimeForColumn)
        {
            _baseTimeForColumn = baseTimeForColumn;
        }
    }
}