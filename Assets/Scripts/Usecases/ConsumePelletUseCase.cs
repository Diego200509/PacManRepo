using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumePelletUseCase : IConsumePelletUseCase
{
    private readonly IGameBoardGateway board;
    private readonly PacManView view;

    public ConsumePelletUseCase(IGameBoardGateway board, PacManView view)
    {
        this.board = board;
        this.view = view;
    }

    public void Execute(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);

        GameObject tileObj = board.GetTileAt(x, y);
        if (tileObj == null) return;

        Tile tile = tileObj.GetComponent<Tile>();
        if (tile == null) return;

        bool consumed = false;

        if (GameBoardView.isPlayerOneUp)
        {
            if (!tile.didConsumePlayerOne && (tile.isPellet || tile.isSuperPellet))
            {
                tile.didConsumePlayerOne = true;
                GameMenu.playerOnePelletsConsumed++;

                GameBoardView.playerOneScore += tile.isSuperPellet ? 50 : 10;
                consumed = true;
            }
        }
        else
        {
            if (!tile.didConsumePlayerTwo && (tile.isPellet || tile.isSuperPellet))
            {
                tile.didConsumePlayerTwo = true;
                GameMenu.playerTwoPelletsConsumed++;

                GameBoardView.playerTwoScore += tile.isSuperPellet ? 50 : 10;
                consumed = true;
            }
        }

        if (consumed)
        {
            tileObj.GetComponent<SpriteRenderer>().enabled = false;
            view.OnPelletConsumed();

            if (tile.isSuperPellet)
            {
                var ghosts = GameObject.FindGameObjectsWithTag("Ghost");
                foreach (var ghost in ghosts)
                {
                    ghost.GetComponent<GhostView>()?.StartFrightenedMode();
                }
            }
        }
    }
}
