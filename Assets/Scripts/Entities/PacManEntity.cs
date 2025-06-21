using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManEntity
{
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
    public Vector2 NextDirection { get; set; }
    public Node CurrentNode { get; set; }
    public Node PreviousNode { get; set; }
    public Node TargetNode { get; set; }
    public float Speed { get; set; }
    public bool CanMove { get; set; }
    public int Level { get; private set; } = 1;

    public Vector2 Size { get; set; } = new Vector2(1, 1);


    public PacManEntity(Node startNode, float startSpeed)
    {
        CurrentNode = startNode;
        PreviousNode = null;
        Position = startNode.transform.position;
        Direction = Vector2.zero;
        NextDirection = Vector2.zero;
        Speed = startSpeed;
        CanMove = true;
        TargetNode = null;
    }

    public void SetLevel(int level)
    {
        Level = level;
        Speed = Mathf.Clamp(5f + level, 5f, 10f);
        Debug.Log($"Speed actual: {Speed}");

    }

    private Node GetInitialTargetNode(Node node, Vector2 dir)
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
