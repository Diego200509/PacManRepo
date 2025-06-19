using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PacManController : MonoBehaviour
{
    private IMovePacManUseCase _moveUseCase;
    private IConsumePelletUseCase _consumePellet; // NUEVO
    private PacManEntity _entity;
    private PacManView _view;

    private bool _isReady = false;

    // NUEVO: se añade consumePellet al método de inicialización
    public void Initialize(
        IMovePacManUseCase moveUseCase,
        PacManEntity entity,
        PacManView view,
        IConsumePelletUseCase consumePellet)
    {
        _moveUseCase = moveUseCase;
        _entity = entity;
        _view = view;
        _consumePellet = consumePellet;
        _isReady = true;
    }

    void Update()
    {
        if (!_isReady || _entity == null || !_entity.CanMove) return;

        // 1) Leer input (presionado individualmente)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _entity.NextDirection = Vector2.right;
            Debug.Log("→ PRESSED");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _entity.NextDirection = Vector2.left;
            Debug.Log("← PRESSED");
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _entity.NextDirection = Vector2.up;
            Debug.Log("↑ PRESSED");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _entity.NextDirection = Vector2.down;
            Debug.Log("↓ PRESSED");
        }


        // 2) Ejecutar lógica de movimiento
        _moveUseCase.Execute(_entity);

        // 3) Consumir pellet y reproducir sonido solo si realmente se consumió
        bool atePellet = _consumePellet.Execute(_entity.Position);

        // 4) Actualizar posición visual SOLO si se está moviendo
        if (transform.position != (Vector3)_entity.Position)
        {
            transform.position = _entity.Position;
            Debug.Log($"Posición visual actualizada: {transform.position}");

            if (atePellet)
                _view.PlayChomp();
            else
                _view.ShowMoving();
        }
        else
        {
            _view.ShowIdle();
        }


        // 5) Orientación
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
