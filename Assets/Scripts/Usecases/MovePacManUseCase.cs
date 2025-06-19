using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MovePacManUseCase : IMovePacManUseCase
{
    private readonly IGameBoardGateway _board;

    [Inject]
    public MovePacManUseCase(IGameBoardGateway board)
    {
        _board = board;
    }

    public void Execute(PacManEntity pacman)
    {
        if (!pacman.CanMove) return;

        // 1. Intentar cambio de dirección anticipado
        if (pacman.NextDirection != pacman.Direction)
        {
            var possible = CanMove(pacman.CurrentNode, pacman.NextDirection);
            if (possible != null)
            {
                pacman.Direction = pacman.NextDirection;
                pacman.TargetNode = possible;
                pacman.PreviousNode = pacman.CurrentNode;
                pacman.CurrentNode = null;
            }
        }

        // 2. Avanzar nodo a nodo
        if (pacman.TargetNode != null && pacman.CurrentNode != pacman.TargetNode)
        {
            Debug.Log($"Current: {pacman.CurrentNode?.transform.position}, Target: {pacman.TargetNode?.transform.position}, Prev: {pacman.PreviousNode?.transform.position}");
            Debug.Log($"Moviendo hacia nodo en: {pacman.TargetNode.transform.position}");
            pacman.Position += pacman.Direction * pacman.Speed * Time.deltaTime;
            Debug.Log($"Posición actual: {pacman.Position} - Velocidad: {pacman.Speed}");

            if (HasOverShotTarget(pacman))
            {
                // 3. Llegó al nodo
                pacman.CurrentNode = pacman.TargetNode;
                pacman.Position = pacman.CurrentNode.transform.position;

                // 3.1 Portal
                GameObject tile = _board.GetTileAt(
                    Mathf.RoundToInt(pacman.Position.x),
                    Mathf.RoundToInt(pacman.Position.y)
                );
                if (tile != null)
                {
                    var portal = tile.GetComponent<Tile>();
                    if (portal != null && portal.isPortal && portal.portalReceiver != null)
                    {
                        pacman.CurrentNode = portal.portalReceiver.GetComponent<Node>();
                        pacman.Position = pacman.CurrentNode.transform.position;
                    }
                }

                // 4. Intentar seguir avanzando en la nueva dirección primero
                Vector2 nuevaDireccion = pacman.NextDirection;
                Node next = CanMove(pacman.CurrentNode, nuevaDireccion);

                // Si no se puede en la nueva dirección, intenta continuar
                if (next == null)
                {
                    nuevaDireccion = pacman.Direction;
                    next = CanMove(pacman.CurrentNode, nuevaDireccion);
                }

                if (next != null)
                {
                    Debug.Log($"Nuevo target encontrado: {next.transform.position}");
                    pacman.PreviousNode = pacman.CurrentNode;
                    pacman.TargetNode = next;
                    pacman.Direction = (next.transform.position - pacman.CurrentNode.transform.position).normalized;
                    pacman.CurrentNode = null;
                }
                else
                {
                    Debug.Log("Sin target válido en la dirección actual.");
                    pacman.Direction = Vector2.zero;
                    pacman.TargetNode = null;
                    pacman.Position = pacman.CurrentNode.transform.position;
                }

                Debug.Log("Nodo alcanzado y actualizado.");
            }
        }

        // 5. Si no tiene TargetNode, intenta default
        if (pacman.TargetNode == null && pacman.CurrentNode != null)
        {
            var fallback = CanMove(pacman.CurrentNode, Vector2.left);
            if (fallback != null)
            {
                pacman.TargetNode = fallback;
                pacman.PreviousNode = pacman.CurrentNode;
                pacman.CurrentNode = null;
                pacman.Direction = Vector2.left;
                Debug.Log("Forzando movimiento hacia la izquierda.");
            }
        }
    }

    private Node CanMove(Node node, Vector2 dir)
    {
        if (node == null) return null;

        for (int i = 0; i < node.neighbors.Length; i++)
        {
            if (node.validDirections[i] == dir)
                return node.neighbors[i];
        }

        return null;
    }

    private bool HasOverShotTarget(PacManEntity pacman)
    {
        if (pacman.TargetNode == null || pacman.PreviousNode == null) {
            Debug.Log("Overshot alcanzado. Posición ajustada al nodo destino.");
            return false;
        }
           

        Vector2 fromPrevToTarget = (Vector2)pacman.TargetNode.transform.position - (Vector2)pacman.PreviousNode.transform.position;
        Vector2 fromPrevToNow = pacman.Position - (Vector2)pacman.PreviousNode.transform.position;
        Debug.Log($"fromPrevToNow: {fromPrevToNow.sqrMagnitude}, fromPrevToTarget: {fromPrevToTarget.sqrMagnitude}");

        return fromPrevToNow.sqrMagnitude >= fromPrevToTarget.sqrMagnitude - 0.01f; // margen
    }
}
