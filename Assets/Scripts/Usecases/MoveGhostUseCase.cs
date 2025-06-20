﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGhostUseCase : IMoveGhostUseCase
{
    readonly IGameBoardGateway _board;

    public MoveGhostUseCase(IGameBoardGateway board)
    {
        _board = board;
    }

    public void Execute(GhostEntity g, Vector2 pacmanPos, Vector2 pacmanDir)
    {
        if (!g.CanMove) return;

        // 1) Actualizar timers y modos
        UpdateModeTimers(g);

        // 2) Calcular tile objetivo según tipo y modo
        Vector2 targetTile = ChooseTargetTile(g, pacmanPos, pacmanDir);

        // 3) Si estamos en un nodo, elegir siguiente vecino
        if (g.CurrentNode != null)
        {
            g.ReleaseTimer += Time.deltaTime;
            var next = CanMove(g.CurrentNode, g.Direction, g, targetTile);
            if (next != null)
            {
                g.NextDirection = g.Direction;
                g.PreviousNode = g.CurrentNode;
                g.TargetNode = next;
                g.CurrentNode = null;
            }
        }

        // 4) Avanzar hacia TargetNode
        if (g.TargetNode != null && g.CurrentNode != g.TargetNode)
        {
            g.Position += g.Direction * g.Speed * Time.deltaTime;

            if (Overshot(g))
            {
                g.CurrentNode = g.TargetNode;
                g.Position = g.CurrentNode.transform.position;
            }
        }
    }

    void UpdateModeTimers(GhostEntity g)
    {
        if (g.CurrentMode != GhostMode.Frightened)
        {
            // Modo Scatter / Chase normal
            g.ModeTimer += Time.deltaTime;

            switch (g.ModeChangeIteration)
            {
                case 1:
                    if (g.CurrentMode == GhostMode.Scatter && g.ModeTimer > g.ScatterModeTimer1)
                    {
                        g.CurrentMode = GhostMode.Chase;
                        g.ModeTimer = 0f;
                    }
                    else if (g.CurrentMode == GhostMode.Chase && g.ModeTimer > g.ChaseModeTimer1)
                    {
                        g.ModeChangeIteration = 2;
                        g.CurrentMode = GhostMode.Scatter;
                        g.ModeTimer = 0f;
                    }
                    break;

                case 2:
                    if (g.CurrentMode == GhostMode.Scatter && g.ModeTimer > g.ScatterModeTimer2)
                    {
                        g.CurrentMode = GhostMode.Chase;
                        g.ModeTimer = 0f;
                    }
                    else if (g.CurrentMode == GhostMode.Chase && g.ModeTimer > g.ChaseModeTimer2)
                    {
                        g.ModeChangeIteration = 3;
                        g.CurrentMode = GhostMode.Scatter;
                        g.ModeTimer = 0f;
                    }
                    break;

                case 3:
                    if (g.CurrentMode == GhostMode.Scatter && g.ModeTimer > g.ScatterModeTimer3)
                    {
                        g.CurrentMode = GhostMode.Chase;
                        g.ModeTimer = 0f;
                    }
                    else if (g.CurrentMode == GhostMode.Chase && g.ModeTimer > g.ChaseModeTimer3)
                    {
                        g.ModeChangeIteration = 4;
                        g.CurrentMode = GhostMode.Scatter;
                        g.ModeTimer = 0f;
                    }
                    break;

                case 4:
                    if (g.CurrentMode == GhostMode.Scatter && g.ModeTimer > g.ScatterModeTimer4)
                    {
                        g.CurrentMode = GhostMode.Chase;
                        g.ModeTimer = 0f;
                    }
                    break;
            }
        }
        else
        {
            // Modo Frightened
            g.FrightenedTimer += Time.deltaTime;

            if (g.FrightenedTimer >= g.FrightenedDuration)
            {
                // regresa al modo anterior
                g.CurrentMode = g.PreviousMode;
                g.FrightenedTimer = 0f;
                g.ModeTimer = 0f;
            }
            // (Aquí podrías copiar tu lógica de parpadeo, cambiando animador entre ghostBlue/ghostWhite)
        }
    }

    Vector2 ChooseTargetTile(GhostEntity g, Vector2 pmPos, Vector2 pmDir)
    {
        switch (g.CurrentMode)
        {
            case GhostMode.Chase:
                switch (g.Type)
                {
                    case GhostType.Red:
                        return new Vector2(
                            Mathf.RoundToInt(pmPos.x),
                            Mathf.RoundToInt(pmPos.y)
                        );

                    case GhostType.Pink:
                        // 4 tiles adelante de Pac-Man
                        var baseTile = new Vector2(
                            Mathf.RoundToInt(pmPos.x),
                            Mathf.RoundToInt(pmPos.y)
                        );
                        return baseTile + pmDir * 4f;

                    case GhostType.Blue:
                        // Calcula el tile de dos adelante y luego vector desde Blinky, etc.
                        var ahead = new Vector2(
                            Mathf.RoundToInt(pmPos.x),
                            Mathf.RoundToInt(pmPos.y)
                        ) + pmDir * 2f;
                        var blinkyPos = GameObject.Find("Ghost_Blinky").transform.localPosition;
                        var blinkyTile = new Vector2(
                            Mathf.RoundToInt(blinkyPos.x),
                            Mathf.RoundToInt(blinkyPos.y)
                        );
                        var v = ahead - blinkyTile;
                        return blinkyTile + v * 2f;

                    case GhostType.Orange:
                        // si distancia < 8 → scatter (home), sino → target Pac-Man
                        var dist = Vector2.Distance(g.Position, pmPos);
                        if (dist > 8f)
                            return new Vector2(
                                Mathf.RoundToInt(pmPos.x),
                                Mathf.RoundToInt(pmPos.y)
                            );
                        else
                            return g.HomeNode.transform.position;
                }
                break;

            case GhostMode.Scatter:
                return g.HomeNode.transform.position;

            case GhostMode.Frightened:
                return new Vector2(
                    Random.Range(0, _board.Width),
                    Random.Range(0, _board.Height)
                );

            case GhostMode.Consumed:
                return g.GhostHouse.transform.position;
        }

        return Vector2.zero;
    }

    Node CanMove(Node from, Vector2 dir, GhostEntity g, Vector2 targetTile)
    {
        // recolecta opciones sin retroceder
        var list = new System.Collections.Generic.List<(Node node, Vector2 d)>();
        for (int i = 0; i < from.neighbors.Length; i++)
        {
            var d = from.validDirections[i];
            if (d == -dir) continue;
            list.Add((from.neighbors[i], d));
        }

        // si solo hay uno
        if (list.Count == 1)
        {
            g.Direction = list[0].d;
            return list[0].node;
        }

        // si hay varias, elige la que minimice la distancia a targetTile
        float best = float.MaxValue;
        Node pick = null;
        foreach (var (node, d) in list)
        {
            float dist = Vector2.Distance(
                node.transform.position,
                targetTile
            );
            if (dist < best)
            {
                best = dist;
                pick = node;
                g.Direction = d;
            }
        }
        return pick;
    }

    bool Overshot(GhostEntity g)
    {
        // convertimos los Vector3 a Vector2 para restar con g.Position
        Vector2 prev = g.PreviousNode.transform.position;
        Vector2 targ = g.TargetNode.transform.position;
        Vector2 pos = g.Position;
        return (pos - prev).sqrMagnitude > (targ - prev).sqrMagnitude;
    }
}