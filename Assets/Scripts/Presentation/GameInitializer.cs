using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInitializer : MonoBehaviour
{
    [Inject] private IMovePacManUseCase _moveUseCase;
    [Inject] private GameBoardView _boardView;
    [Inject] private PacManView _pacManView;
    [Inject] private DiContainer _container;

    void Start()
    {
        StartCoroutine(InitializeAfterBoardIsReady());
    }

    IEnumerator InitializeAfterBoardIsReady()
    {
        yield return null;

        Vector3 pos = _pacManView.transform.position;
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        var tile = _boardView.GetTileAt(x, y);
        if (tile == null)
        {
            Debug.LogError($"Tile at position ({x},{y}) not found. Board may not be initialized yet.");
            yield break;
        }

        var node = tile.GetComponent<Node>();
        if (node == null)
        {
            Debug.LogError("Node component missing on tile.");
            yield break;
        }

        float speed = _pacManView.speed;
        var entity = new PacManEntity(node, speed);

        // Registramos la entidad en el contenedor
        _container.Bind<PacManEntity>().FromInstance(entity).AsSingle();

        // Inyección manual del PacManController
        var controller = _pacManView.GetComponent<PacManController>();
        if (controller != null)
        {
            controller.Initialize(_moveUseCase, entity, _pacManView);
            Debug.Log("PacManController inyectado manualmente con éxito.");
        }
        else
        {
            Debug.LogError("PacManController no encontrado en el GameObject de PacManView.");
        }

        Debug.Log("PacManEntity creado y registrado correctamente.");
    }
}
