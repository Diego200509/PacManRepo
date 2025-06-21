using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICheckCollisionUseCase
{
    void Execute(PacManEntity pacMan, GhostEntity ghost, IGameBoardGateway gameBoard);
}
