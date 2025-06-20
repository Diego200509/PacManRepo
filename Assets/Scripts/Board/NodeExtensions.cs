using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodeExtensions
{
    /// <summary>
    /// Devuelve el nodo vecino en la dirección indicada, si existe.
    /// </summary>
    public static Node GetNeighborInDirection(this Node node, Vector2 dir)
    {
        for (int i = 0; i < node.validDirections.Length; i++)
        {
            if (node.validDirections[i] == dir)
                return node.neighbors[i];
        }
        return null;
    }
}
