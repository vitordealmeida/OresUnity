namespace Domain.Entities
{
    public interface Level
    {
        float TimeForColumnAdd { get; }
        float ScoreMultiplier { get; }
        int OreTypeCount { get; }
        int ScoreLimit { get; }
    }
}