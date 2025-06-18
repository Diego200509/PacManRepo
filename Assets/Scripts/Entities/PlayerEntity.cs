public class PlayerEntity
{
    public string Nombre { get; }
    public int MaxScore { get; private set; }
    public int LastScore { get; private set; }
    public int MaxLevel { get; private set; }
    public int LastLevel { get; private set; }
    public PlayerEntity(string nombre, int maxScore = 0, int lastScore = 0, int maxLevel = 0, int lastLevel = 0)
    {
        Nombre = nombre;
        MaxScore = maxScore;
        LastScore = lastScore;
        LastLevel = lastLevel;
        MaxLevel = maxLevel;
    }

    public void UpdateScore(int score)
    {
        this.LastScore = score;
        this.MaxScore = score > MaxScore ? score : MaxScore;
    }

    public void UpdateLevel(int level)
    {
        this.LastLevel = level;
        this.MaxLevel = level > MaxLevel ? level : MaxLevel;
    }
}
