using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class LogoutAction : MonoBehaviour
{
    [SerializeField] private Button salirButton;

    private LogoutPlayer _logoutPlayer;
    [Inject]
    public void Construct(
        LogoutPlayer logoutPlayer
    )
    {
        _logoutPlayer = logoutPlayer;
    }
    // Start is called before the first frame update
    private void Start()
    {
        salirButton.onClick.AddListener(OnSalirClick);
    }

    private void OnSalirClick()
    {
        try
        {
            PlayerEntity player = SessionEntity.GetInstance().CurrentPlayer;
            if (player == null)
            {
                Debug.LogError("Skibidi: No se ha iniciado sesion");
            }
            _logoutPlayer.DestroySession();
            SceneManager.LoadScene("Login");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error al hacer algo. ESTOY EN LOGOUTACTION {ex.Message}");
        }
    }
}
