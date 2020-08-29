namespace Domain.Entities
{
    public class Level1 : Level
    {
        private readonly float _baseTimeForColumn;

        public float TimeForColumnAdd => _baseTimeForColumn;
        public float ScoreMultiplier => 1;
        public int OreTypeCount => 4;
        public int ScoreLimit => 200;

        public Level1(float baseTimeForColumn)
        {
            _baseTimeForColumn = baseTimeForColumn;
        }
    }
}