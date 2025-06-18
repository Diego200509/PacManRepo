public class LoginPlayer : ISetPlayerSession
{
    public IDatabase<PlayerEntity> Database { get; set; }
    public LoginPlayer(IDatabase<PlayerEntity> database)
    {
        Database = database;
    }
    public void SetSession(string name)
    {
        PlayerEntity current = this.Database.FindByName(name);
        SessionEntity.CreateSession(current);
    }

}
