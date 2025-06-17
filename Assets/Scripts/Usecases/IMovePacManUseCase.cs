using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Caso de uso: desplazar Pac-Man en la dirección indicada.
/// </summary>
public interface IMovePacManUseCase
{
    void Execute(PacManEntity pacman);
}
