using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LoginAction : MonoBehaviour
{
    [SerializeField] private InputField nombreInput;
    [SerializeField] private Button ingresarButton;

    private IDatabase<PlayerEntity> _database;
    private LoginPlayer _loginUseCase;
    private RegisterPlayer _registerUseCase;

    [Inject]
    public void Construct(
        IDatabase<PlayerEntity> database,
        LoginPlayer loginUseCase,
        RegisterPlayer registerUseCase)
    {
        _database = database;
        _loginUseCase = loginUseCase;
        _registerUseCase = registerUseCase;
    }

    private void Start()
    {
        ingresarButton.onClick.AddListener(OnIngresarClick);
    }

    private void OnIngresarClick()
    {
        string nombre = nombreInput.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            Debug.LogWarning("⚠️ El nombre no puede estar vacío.");
            return;
        }

        try
        {
            var jugadorExistente = _database.FindByName(nombre);

            if (jugadorExistente != null)
            {
                _loginUseCase.SetSession(nombre);
                Debug.Log($"✅ Bienvenido nuevamente, {nombre}.");
            }
            else
            {
                _registerUseCase.SetSession(nombre);
                Debug.Log($"🆕 Usuario {nombre} creado correctamente.");
            }

            SceneManager.LoadScene("Level1");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Error al iniciar sesión: {ex.Message}");
        }
    }
}
