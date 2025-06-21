using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GhostInitializer : MonoBehaviour
{
    [Inject] private IGameBoardGateway _board;
    [Inject] private IMoveGhostUseCase _moveGhostUseCase;
    [Inject] private ICheckCollisionUseCase _collisionUseCase;
    [Inject] private DiContainer _container;
    private PacManEntity _pacEntity;
    public void SetPacManEntity(PacManEntity entity)
    {
        _pacEntity = entity;
    }

    void Start()
    {
        StartCoroutine(InitializeGhosts());
    }

    IEnumerator InitializeGhosts()
    {
        yield return null;

        yield return new WaitForSeconds(3f);

        var ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (var ghostGO in ghosts)
        {
            var view = ghostGO.GetComponent<GhostView>();
            var controller = ghostGO.GetComponent<GhostController>();

            if (view == null || controller == null)
            {
                Debug.LogError("GhostView o GhostController faltan en " + ghostGO.name);
                continue;
            }

            Vector3 pos = ghostGO.transform.position;
            var tile = _board.GetTileAt(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            var node = tile?.GetComponent<Node>();

            if (node == null)
            {
                Debug.LogError("Nodo no encontrado para " + ghostGO.name);
                continue;
            }

            // Detectar tipo por nombre
            GhostType type = GhostType.Red;
            Node home = node;
            Node house = node;
            Debug.Log($"Ghost {ghostGO.name} house node: {house?.name}");

            if (ghostGO.name.Contains("Pinky")) type = GhostType.Pink;
            else if (ghostGO.name.Contains("Inky")) type = GhostType.Blue;
            else if (ghostGO.name.Contains("Clyde")) type = GhostType.Orange;

            int level = GameBoardView.isPlayerOneUp ? GameBoardView.playerOneLevel : GameBoardView.playerTwoLevel;

            var entity = new GhostEntity(
                node,
                view.speed,
                type,
                home,
                house,
                frightenedDuration: 10f,
                startBlinkingAt: 7f,
                pinkyRelease: 5f,
                inkyRelease: 5f,
                clydeRelease: 21f,
                scatter1: 3f, chase1: 10f,
                scatter2: 3f, chase2: 10f,
                scatter3: 3f, chase3: 10f,
                scatter4: 3f
            );

            // Establecer modo y dirección iniciales
            entity.CurrentMode = GhostMode.Scatter;
            entity.PreviousMode = GhostMode.Scatter;
            entity.Direction = Vector2.left;

            // Establecer primer nodo objetivo si posible
            var nextNode = node.GetNeighborInDirection(Vector2.left); 
            if (nextNode != null)
            {
                entity.TargetNode = nextNode;
                entity.PreviousNode = node;
            }

            // Inyectar y registrar
            _container.Inject(controller);
            _container.Bind<GhostEntity>().FromInstance(entity).AsTransient();
            controller.GetType().GetField("_entity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, entity);
            controller.Initialize(
                _moveGhostUseCase,
                entity,
                _pacEntity,
                _collisionUseCase,
                _board
            );

        }

        Debug.Log("Todos los fantasmas inicializados correctamente");
    }
}
