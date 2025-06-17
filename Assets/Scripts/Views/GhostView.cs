using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class GhostView : MonoBehaviour
{
    [Header("Chase/Scatter Animations")]
    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;

    [Header("Frightened Animations")]
    public RuntimeAnimatorController ghostBlue;
    public RuntimeAnimatorController ghostWhite;

    [Header("Consumed Eyes Sprites")]
    public Sprite eyesUp;
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;

    [Header("Movement (speed display only)")]
    public float speed = 5.9f;
    [HideInInspector]
    public bool canMove = true;

    // Para teletransportar de vuelta al reiniciar nivel
    Vector3 _startLocalPos;

    Animator _anim;
    SpriteRenderer _sprite;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _startLocalPos = transform.localPosition;
    }

    /// <summary>
    /// Teletransporta al punto de inicio y oculta el sprite
    /// (usado por GameBoardView al reiniciar o al comenzar un nivel).
    /// </summary>
    public void MoveToStartingPosition()
    {
        transform.localPosition = _startLocalPos;
        _sprite.enabled = false;
        _anim.enabled = false;
        canMove = false;
    }

    /// <summary>
    /// Ajusta la “velocidad” (solo para que la veas en el Inspector).
    /// </summary>
    public void SetDifficultyForLevel(int level)
    {
        switch (level)
        {
            case 1: speed = 5.9f; break;
            case 2: speed = 6.9f; break;
            case 3: speed = 7.9f; break;
            case 4: speed = 8.9f; break;
            case 5: speed = 9.9f; break;
            default: speed = 5.9f; break;
        }
    }

    /// <summary>
    /// Vuelve al estado “activo” inicial: muestra el sprite y la animación normal.
    /// </summary>
    public void Restart()
    {
        _sprite.enabled = true;
        _anim.runtimeAnimatorController = ghostLeft;
        _anim.enabled = true;
        canMove = true;
    }

    /// <summary>
    /// Scatter y Chase usan la misma animación, solo varia la dirección.
    /// </summary>
    public void SetScatterOrChase(Vector2 dir)
    {
        _sprite.enabled = true;
        _anim.enabled = true;

        if (dir == Vector2.up) _anim.runtimeAnimatorController = ghostUp;
        else if (dir == Vector2.down) _anim.runtimeAnimatorController = ghostDown;
        else if (dir == Vector2.left) _anim.runtimeAnimatorController = ghostLeft;
        else if (dir == Vector2.right) _anim.runtimeAnimatorController = ghostRight;
        else _anim.runtimeAnimatorController = ghostLeft;
    }

    /// <summary>
    /// En frightened mode alterna entre azul y blanco.
    /// </summary>
    public void SetFrightened(bool white)
    {
        _sprite.enabled = true;
        _anim.enabled = true;
        _anim.runtimeAnimatorController = white ? ghostWhite : ghostBlue;
    }

    /// <summary>
    /// En modo “consumed” sólo se ven los ojos apuntando en la dirección dada.
    /// </summary>
    public void SetConsumedEyes(Vector2 dir)
    {
        _anim.enabled = false; // sin animación, solo sprite
        _sprite.enabled = true;

        if (dir == Vector2.up) _sprite.sprite = eyesUp;
        else if (dir == Vector2.down) _sprite.sprite = eyesDown;
        else if (dir == Vector2.left) _sprite.sprite = eyesLeft;
        else if (dir == Vector2.right) _sprite.sprite = eyesRight;
        else _sprite.sprite = eyesLeft;
    }
}