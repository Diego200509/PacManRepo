using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GhostInitializer : MonoBehaviour
{
    [Inject] private IGameBoardGateway _board;
    [Inject] private IMoveGhostUseCase _moveGhostUseCase;
    [Inject] private DiContainer _container;

    void Start()
    {
        StartCoroutine(InitializeGhosts());
    }

    IEnumerator InitializeGhosts()
    {
        yield return null;

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

            // Define tipo y parámetros según nombre
            GhostType type = GhostType.Red;
            Node home = node;
            Node house = node;

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
                inkyRelease: 14f,
                clydeRelease: 21f,
                scatter1: 7f, chase1: 20f,
                scatter2: 7f, chase2: 20f,
                scatter3: 5f, chase3: 20f,
                scatter4: 5f
            );

            // Inyectar entidad al controlador
            _container.Inject(controller); // para asegurar que se inyecten las dependencias
            _container.Bind<GhostEntity>().FromInstance(entity).AsTransient(); 
            controller.GetType().GetField("_entity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, entity);
            controller.Initialize(_moveGhostUseCase, entity);
        }

        Debug.Log("Todos los fantasmas inicializados");
    }
}
