using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GhostMode { Scatter, Chase, Frightened, Consumed }
public enum GhostType { Red, Pink, Blue, Orange }

public class GhostEntity
{
    // --- Estado de posición y navegación ---
    public Vector2 Position { get; set; }
    public Vector2 Direction { get; set; }
    public Vector2 NextDirection { get; set; }
    public Node CurrentNode { get; set; }
    public Node PreviousNode { get; set; }
    public Node TargetNode { get; set; }

    // --- Configuración de velocidad y movimiento ---
    public float Speed { get; set; }
    public bool CanMove { get; set; }
    public float NormalSpeed { get; set; }


    // --- Modo y temporizadores de cambio de modo ---
    public GhostMode CurrentMode { get; set; }
    public GhostMode PreviousMode { get; set; }
    public float ModeTimer { get; set; }

    // --- Modo frightened ---
    public float FrightenedTimer { get; set; }
    public float FrightenedDuration { get; set; }
    public float StartBlinkingAt { get; set; }

    // --- Modo scatter/chase (4 fases) ---
    public float ScatterModeTimer1 { get; set; }
    public float ChaseModeTimer1 { get; set; }
    public float ScatterModeTimer2 { get; set; }
    public float ChaseModeTimer2 { get; set; }
    public float ScatterModeTimer3 { get; set; }
    public float ChaseModeTimer3 { get; set; }
    public float ScatterModeTimer4 { get; set; }

    // --- Fantasma dentro / fuera de la casa ---
    public bool IsInGhostHouse { get; set; }
    public float ReleaseTimer { get; set; }
    public float PinkyReleaseTimer { get; set; }
    public float InkyReleaseTimer { get; set; }
    public float ClydeReleaseTimer { get; set; }
    public int ModeChangeIteration { get; set; }

    // --- Referencias a nodos especiales ---
    public Node HomeNode { get; set; }
    public Node GhostHouse { get; set; }

    // --- Tipo y constructor básico ---
    public GhostType Type { get; }

    public GhostEntity(
        Node startNode,
        float startSpeed,
        GhostType type,
        Node homeNode,
        Node ghostHouse,
        // timers:
        float frightenedDuration,
        float startBlinkingAt,
        float pinkyRelease,
        float inkyRelease,
        float clydeRelease,
        float scatter1, float chase1,
        float scatter2, float chase2,
        float scatter3, float chase3,
        float scatter4
    )
    {
        // navegación
        CurrentNode = startNode;
        PreviousNode = null;
        TargetNode = startNode;
        Position = startNode.transform.position;
        Direction = Vector2.left;
        NextDirection = Vector2.left;

        // movimiento
        Speed = startSpeed;
        CanMove = true;
        NormalSpeed = startSpeed;

        // modos y timers
        CurrentMode = GhostMode.Scatter;
        PreviousMode = GhostMode.Scatter;
        ModeTimer = 0f;
        ModeChangeIteration = 1;

        FrightenedDuration = frightenedDuration;
        StartBlinkingAt = startBlinkingAt;
        FrightenedTimer = 0f;

        ScatterModeTimer1 = scatter1;
        ChaseModeTimer1 = chase1;
        ScatterModeTimer2 = scatter2;
        ChaseModeTimer2 = chase2;
        ScatterModeTimer3 = scatter3;
        ChaseModeTimer3 = chase3;
        ScatterModeTimer4 = scatter4;

        IsInGhostHouse = false;
        PinkyReleaseTimer = pinkyRelease;
        InkyReleaseTimer = inkyRelease;
        ClydeReleaseTimer = clydeRelease;
        ReleaseTimer = 0f;

        HomeNode = homeNode;
        GhostHouse = ghostHouse;

        Type = type;
    }
    public void ResetToStart()
    {
        CurrentNode = PreviousNode = TargetNode = null;
        Position = Vector2.zero;
        Direction = Vector2.left;
        NextDirection = Vector2.left;
        ModeTimer = FrightenedTimer = ReleaseTimer = 0f;
        ModeChangeIteration = 1;
        CurrentMode = GhostMode.Scatter;
        CanMove = true;
    }

}
