using Zenject;

public class PlayerSessionProvider : IPlayerSessionProvider
{

    private readonly IDatabase<PlayerEntity> _database;
    private readonly DiContainer _container;

    public PlayerSessionProvider(
        IDatabase<PlayerEntity> database,
        DiContainer container
    )
    {
        _database = database;
        _container = container;
    }
    public ISetPlayerSession GetPlayerSession(string player)
    {
        var found = _database.FindByName(player);
        if (found != null)
        {
            return _container.Instantiate<LoginPlayer>();
        }
        else
        {
            return _container.Instantiate<RegisterPlayer>();
        }
    }

}
