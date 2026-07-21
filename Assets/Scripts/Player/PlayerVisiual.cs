using UnityEngine;

public class PlayerVisual : MonoBehaviour
{

    private static readonly int Running = Animator.StringToHash(IS_RUNNING);
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private const string IS_RUNNING = "IsRunning";


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();


        if (_animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }
    }



    private void Update()
    {
        // Защита: если PlayerMovement ещё не создан или уже уничтожен —
        // пропускаем кадр вместо падения с MissingReferenceException.
        if (PlayerMovement.Instance == null) return;

        _animator.SetBool(Running, PlayerMovement.Instance.IsRunning());

        if (PlayerMovement.Instance.IsAlive())
        {
            AdjustPlayerFacingDirection();
        }
    }
    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = GameInput.GetMousePosition();
        Vector3 playerPosition = PlayerMovement.Instance.GetPlayerScreenPosition();
        _spriteRenderer.flipX = mousePos.x < playerPosition.x;
    }

}