public class RegisterPlayer : ISetPlayerSession
{
    public IDatabase<PlayerEntity> Database { get; set; }
    public RegisterPlayer(IDatabase<PlayerEntity> database)
    {
        Database = database;
    }
    public void SetSession(string name)
    {
        PlayerEntity player = new PlayerEntity(name);
        Database.Add(player);
        SessionEntity.CreateSession(player);
    }

}
