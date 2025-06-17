using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cada frame decide el siguiente movimiento del fantasma.
/// </summary>
public interface IMoveGhostUseCase
{
    void Execute(GhostEntity ghost, Vector2 pacmanPosition, Vector2 pacmanDirection);
}
