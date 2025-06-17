using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Puerta de acceso al tablero de juego.
/// </summary>
public interface IGameBoardGateway
{
    int Width { get; }
    int Height { get; }

    GameObject GetTileAt(int x, int y);
    void SetTileAt(int x, int y, GameObject obj);
}
