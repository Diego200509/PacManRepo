using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MovePacManUseCase : IMovePacManUseCase
{
    IGameBoardGateway _board;

    [Inject]
    public MovePacManUseCase(IGameBoardGateway board)
    {
        _board = board;
    }

    public void Execute(PacManEntity pacman)
    {
        // Cambiar de dirección si es posible
        if (pacman.NextDirection != pacman.Direction)
        {
            var nextNode = CanMove(pacman.CurrentNode, pacman.NextDirection);
            if (nextNode != null)
            {
                pacman.Direction = pacman.NextDirection;
                pacman.TargetNode = nextNode;
                pacman.PreviousNode = pacman.CurrentNode;
                pacman.CurrentNode = null;
            }
        }

        // Avanzar hacia targetNode
        if (pacman.TargetNode != null && pacman.CurrentNode != pacman.TargetNode)
        {
            // mover posición
            var step = pacman.Direction * pacman.Speed * Time.deltaTime;
            pacman.Position += step;

            // comprobar si superó el target
            if (Vector2.SqrMagnitude((Vector2)pacman.Position - (Vector2)pacman.TargetNode.transform.position) <
                Vector2.SqrMagnitude((Vector2)pacman.PreviousNode.transform.position - (Vector2)pacman.TargetNode.transform.position))
            {
                // llegó o superó el nodo
                pacman.CurrentNode = pacman.TargetNode;
                pacman.Position = pacman.CurrentNode.transform.position;
            }
        }

        //Actualizar la entidad restableciendo target si estamos en nodo
        if (pacman.CurrentNode != null)
        {
            var next = CanMove(pacman.CurrentNode, pacman.Direction);
            if (next != null)
            {
                pacman.PreviousNode = pacman.CurrentNode;
                pacman.TargetNode = next;
                pacman.CurrentNode = null;
            }
            else
            {
                pacman.Direction = Vector2.zero; 
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
}
