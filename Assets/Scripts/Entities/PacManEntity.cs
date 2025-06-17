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

    public PacManEntity(Node startNode, float startSpeed)
    {
        CurrentNode = startNode;
        PreviousNode = null;
        TargetNode = startNode;
        Position = startNode.transform.position;
        Direction = Vector2.left;
        NextDirection = Vector2.left;
        Speed = startSpeed;
        CanMove = true;
    }
}
