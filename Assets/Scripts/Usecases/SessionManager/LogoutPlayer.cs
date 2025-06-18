public class LogoutPlayer : IDestroyPlayerSession
{
    public IDatabase<PlayerEntity> Database { get; set; }
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
