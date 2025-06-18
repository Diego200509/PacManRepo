public class PlayerEntity
{
    public string Nombre { get; }
    public int MaxScore { get; private set; }
    public int LastScore { get; private set; }
    public PlayerEntity(string nombre, int maxScore = 0, int lastScore = 0)
    {
        Nombre = nombre;
        MaxScore = maxScore;
        LastScore = lastScore;
    }

    public void UpdateScore(int score)
    {
        this.LastScore = score;
        this.MaxScore = score > MaxScore ? score : MaxScore;
    }
}
