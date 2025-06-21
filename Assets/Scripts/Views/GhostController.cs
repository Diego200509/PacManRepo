using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(
    typeof(SpriteRenderer),
    typeof(Animator),
    typeof(GhostView)
)]
public class GhostController : MonoBehaviour
{
    private IMoveGhostUseCase _moveUseCase;
    private ICheckCollisionUseCase _collisionUseCase;
    private IGameBoardGateway _gameBoard;
    private PacManEntity _pacEntity;
    private GhostEntity _entity;

    private GhostView _view;
    private Transform _t;
    private PacManView _pacView;

    void Awake()
    {
        _t = transform;
        _view = GetComponent<GhostView>();

        // Encontramos la vista de Pac-Man para leer su orientación
        var pacObj = GameObject.FindWithTag("PacMan");
        if (pacObj != null)
            _pacView = pacObj.GetComponent<PacManView>();
    }

    void Update()
    {
        if (_entity == null || !_entity.CanMove) return;

        // 1) Obtenemos posición y dirección de Pac-Man desde su View
        Vector2 pmPos = Vector2.zero;
        Vector2 pmDir = Vector2.zero;
        if (_pacView != null)
        {
            pmPos = _pacView.transform.localPosition;
            pmDir = _pacView.orientation;
        }

        // 2) Ejecutamos la lógica de movimiento
        _moveUseCase.Execute(_entity, pmPos, pmDir);

        // 3) Aplicamos la nueva posición al Transform
        _t.localPosition = _entity.Position;

        // 4) Actualizamos sprites y animaciones
        ApplyVisuals();

        // 5) Actualizamos posición en entidad y verificamos colisión
        _entity.Position = _t.localPosition;
        _collisionUseCase.Execute(_pacEntity, _entity, _gameBoard);
    }

    void ApplyVisuals()
    {
        switch (_entity.CurrentMode)
        {
            case GhostMode.Scatter:
            case GhostMode.Chase:
                _view.SetScatterOrChase(_entity.Direction);
                break;

            case GhostMode.Frightened:
                bool white = _entity.FrightenedTimer >= _entity.StartBlinkingAt
                             && ((int)(_entity.FrightenedTimer * 10) % 2 == 0);
                _view.SetFrightened(white);
                break;

            case GhostMode.Consumed:
                _view.SetConsumedEyes(_entity.Direction);
                break;
        }
    }

    public void Initialize(
       IMoveGhostUseCase moveUseCase,
       GhostEntity entity,
       PacManEntity pacEntity,
       ICheckCollisionUseCase collisionUseCase,
       IGameBoardGateway gameBoard)
    {
        _moveUseCase = moveUseCase;
        _entity = entity;
        _pacEntity = pacEntity;
        _collisionUseCase = collisionUseCase;
        _gameBoard = gameBoard;
    }

    public GhostEntity GetEntity()
    {
        return _entity;
    }
}
