using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ConsumePelletUseCase : IConsumePelletUseCase
{
    private readonly IGameBoardGateway board;
    private readonly PacManView view;

    [Inject]
    public ConsumePelletUseCase(IGameBoardGateway board, PacManView view)
    {
        this.board = board;
        this.view = view;
    }

    public bool Execute(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x + 0.5f);
        int y = Mathf.FloorToInt(position.y + 0.5f);

        GameObject tileObj = board.GetTileAt(x, y);
        if (tileObj == null) return false;

        Tile tile = tileObj.GetComponent<Tile>();
        if (tile == null) return false;

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
            view.OnPelletConsumed(); // Animación visual u otros efectos
            view.IncreaseSpeedTemporarily(2f, 1.5f); // por ejemplo

            if (tile.isSuperPellet)
            {
                var ghosts = GameObject.FindGameObjectsWithTag("Ghost");
                foreach (var ghost in ghosts)
                {
                    var entity = ghost.GetComponent<GhostController>()?.GetEntity();
                    if (entity != null && entity.CurrentMode != GhostMode.Consumed)
                    {
                        entity.PreviousMode = entity.CurrentMode;
                        entity.CurrentMode = GhostMode.Frightened;
                        entity.FrightenedTimer = 0f;

                        ghost.GetComponent<GhostView>()?.StartFrightenedMode();
                    }
                }
            }
        }

        return consumed;
    }
}
