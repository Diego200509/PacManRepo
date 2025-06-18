public class SessionEntity
{
    public PlayerEntity CurrentPlayer { get; private set; }
    private static SessionEntity _instance;

    private SessionEntity(PlayerEntity player)
    {
        CurrentPlayer = player;
    }
    public static void CreateSession(PlayerEntity player)
    {
        if (_instance != null)
            throw new System.Exception("Sesion ya iniciada. VENGO DE SESSIONENTITY");

        if (player == null)
            throw new System.Exception("Mal jugador para inicializar. VENGO DE SESSIONENTITY");

        _instance = new SessionEntity(player);
    }
    public static SessionEntity GetInstance()
    {
        if (_instance == null)
        {
            throw new System.Exception("Sesión no iniciada. VENGO DE SESSIONENTITY");
        }
        return _instance;
    }
    public static void DestroySession()
    {
        _instance = null;
    }
}
