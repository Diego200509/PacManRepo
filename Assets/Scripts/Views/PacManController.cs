using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PacManController : MonoBehaviour
{
    private IMovePacManUseCase _moveUseCase;
    private PacManEntity _entity;
    private PacManView _view;

    private bool _isReady = false;

    public void Initialize(IMovePacManUseCase moveUseCase, PacManEntity entity, PacManView view)
    {
        _moveUseCase = moveUseCase;
        _entity = entity;
        _view = view;
        _isReady = true;
    }

    void Update()
    {
        if (!_isReady || _entity == null || !_entity.CanMove) return;

        // 1) Leer input
        _entity.NextDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // 2) Ejecutar lógica
        _moveUseCase.Execute(_entity);

        // 3) Actualizar vista
        transform.localPosition = _entity.Position;

        if (_entity.Direction == Vector2.zero)
            _view.ShowIdle();
        else
            _view.PlayChomp();

        UpdateOrientation();
    }

    void UpdateOrientation()
    {
        var dir = _entity.Direction;
        if (dir == Vector2.left)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (dir == Vector2.right)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dir == Vector2.up)
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        else if (dir == Vector2.down)
            transform.localRotation = Quaternion.Euler(0, 0, 270);
    }
}
