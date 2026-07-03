using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    
    private static readonly int Running = Animator.StringToHash(IsRunning);
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
  
    private const string IsRunning = "IsRunning";
    

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