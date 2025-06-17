using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource), typeof(Animator), typeof(SpriteRenderer))]
public class PacManView : MonoBehaviour
{
    [Header("Chomp SFX")]
    public AudioClip chomp1;
    public AudioClip chomp2;

    [Header("Animations")]
    public RuntimeAnimatorController chompAnimation;
    public RuntimeAnimatorController deathAnimation;

    [Header("Idle Sprite")]
    public Sprite idleSprite;

    [Header("Movement (speed config only)")]
    public float speed = 6f;

    // --- Añadido para que GhostController pueda saber hacia dónde mira Pac-Man
    [HideInInspector]
    public Vector2 orientation = Vector2.left;

    // --- Mantiene si puede moverse o no (usado por GameBoardView) ---
    [HideInInspector]
    public bool canMove = true;

    // componente cacheado
    AudioSource _audio;
    Animator _anim;
    SpriteRenderer _sprite;

    // guardamos la posición inicial para MoveToStartingPosition()
    Vector3 _startLocalPos;

    // para alternar chomp1/chomp2
    bool _playedChomp1 = false;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        _startLocalPos = transform.localPosition;
    }

    /// <summary>
    /// Reproduce un chomp alternando entre clip1 y clip2
    /// </summary>
    public void PlayChomp()
    {
        var clip = _playedChomp1 ? chomp2 : chomp1;
        _playedChomp1 = !_playedChomp1;
        _audio.PlayOneShot(clip);

        _anim.runtimeAnimatorController = chompAnimation;
        _anim.enabled = true;
    }

    /// <summary>
    /// Entra en estado idle (sprite estático)
    /// </summary>
    public void ShowIdle()
    {
        _anim.enabled = false;
        _sprite.sprite = idleSprite;
    }

    /// <summary>
    /// Reproduce la animación de muerte
    /// </summary>
    public void PlayDeath()
    {
        _anim.runtimeAnimatorController = deathAnimation;
        _anim.enabled = true;
    }

    // --- Estos métodos los invoca tu GameBoardView directamente: ---

    /// <summary>
    /// Teletransporta a la posición inicial y pasa a idle.
    /// </summary>
    public void MoveToStartingPosition()
    {
        transform.localPosition = _startLocalPos;
        ShowIdle();
        canMove = false;
    }

    /// <summary>
    /// Ajusta la “velocidad” (solo para que tú veas el valor en el Inspector)
    /// </summary>
    public void SetDifficultyForLevel(int level)
    {
        // tal como lo hacías antes
        switch (level)
        {
            case 1: speed = 6f; break;
            case 2: speed = 7f; break;
            case 3: speed = 8f; break;
            case 4: speed = 9f; break;
            case 5: speed = 10f; break;
            default: speed = 6f; break;
        }
    }

    /// <summary>
    /// “Resetea” la vista como al inicio de la partida
    /// </summary>
    public void Restart()
    {
        canMove = true;
        _anim.runtimeAnimatorController = chompAnimation;
        _anim.enabled = true;
        ShowIdle();
    }

    // --- Opcional: si quieres centralizar la orientación de la vista aquí ---
    /// <summary>
    /// Llama desde tu PacManController tras mover la entidad,
    /// para reflejar la dirección en la vista.
    /// </summary>
    public void UpdateOrientation(Vector2 dir)
    {
        orientation = dir;

        if (dir == Vector2.left) transform.localScale = new Vector3(-1, 1, 1);
        else if (dir == Vector2.right) transform.localScale = Vector3.one;
        else if (dir == Vector2.up)
        {
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (dir == Vector2.down)
        {
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.Euler(0, 0, 270);
        }
    }
}