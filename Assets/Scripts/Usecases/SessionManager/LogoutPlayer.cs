public class LogoutPlayer : IDestroyPlayerSession
{
    public readonly IDatabase<PlayerEntity> Database;
    public LogoutPlayer(IDatabase<PlayerEntity> database)
    {
        Database = database;
    }
    public void DestroySession()
    {
        PlayerEntity currentPlayer = SessionEntity.GetInstance().CurrentPlayer;
        Database.Update(currentPlayer);
        SessionEntity.DestroySession();
    }

}
