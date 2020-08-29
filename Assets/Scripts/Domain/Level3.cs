namespace Domain.Entities
{
    public class Level3 : Level
    {
        private readonly float _baseTimeForColumn;

        public float TimeForColumnAdd => _baseTimeForColumn * .75f;
        public float ScoreMultiplier => 2f;
        public int OreTypeCount => 5;
        public int ScoreLimit => 3000;

        public Level3(float baseTimeForColumn)
        {
            _baseTimeForColumn = baseTimeForColumn;
        }
    }
}