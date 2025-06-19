using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInitializer : MonoBehaviour
{
    [Inject] private IMovePacManUseCase _moveUseCase;
    [Inject] private IConsumePelletUseCase _consumePelletUseCase; 
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

        Debug.Log($"StartNode: {node.transform.position}, vecinos: {node.neighbors.Length}");

        for (int i = 0; i < node.validDirections.Length; i++)
        {
            Debug.Log($"Vecino {i}: Dir {node.validDirections[i]}, Nodo en: {node.neighbors[i]?.transform.position}");
        }

        var entity = new PacManEntity(node, _pacManView.speed);
        entity.SetLevel(GameBoardView.isPlayerOneUp ? GameBoardView.playerOneLevel : GameBoardView.playerTwoLevel);

        // Asegura que el TargetNode inicial sea válido
        var firstTarget = GetInitialTargetNode(node, Vector2.left);
        if (firstTarget == null)
        {
            Debug.LogWarning("No se pudo encontrar un nodo vecino hacia la izquierda. Intentando hacia la derecha...");
            firstTarget = GetInitialTargetNode(node, Vector2.right);
        }

        if (firstTarget == null)
        {
            Debug.LogError("No se pudo encontrar un nodo vecino inicial válido.");
            yield break;
        }

        entity.TargetNode = firstTarget;
        entity.PreviousNode = node;

        _pacManView.transform.localPosition = entity.Position;

        // Registrar entidad en DI
        _container.Bind<PacManEntity>().FromInstance(entity).AsSingle();

        var controller = _pacManView.GetComponent<PacManController>();
        if (controller != null)
        {
            controller.Initialize(_moveUseCase, entity, _pacManView, _consumePelletUseCase);
            Debug.Log("PacManController inyectado manualmente con éxito.");
        }
        else
        {
            Debug.LogError("PacManController no encontrado en el GameObject de PacManView.");
        }

        Debug.Log("PacManEntity creado y registrado correctamente.");
    }

    // Método para obtener un nodo vecino inicial
    private Node GetInitialTargetNode(Node node, Vector2 direction)
    {
        if (node == null) return null;

        for (int i = 0; i < node.neighbors.Length; i++)
        {
            if (node.validDirections[i] == direction)
                return node.neighbors[i];
        }

        return null;
    }
}
