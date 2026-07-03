using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private PlayerInputActions input;
    public static PlayerMovement Instance { get; private set; }
    private Camera _mainCamera;

    private bool _isAlive;
    private bool _isRunning;
    /* private InteractionHint currentHint; */

    private void Awake()
    {
        Instance = this;
         _isAlive = true;
        rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        Debug.Log($"{gameObject.name} → input создан");
        input = new PlayerInputActions();
        input.Player.Enable();
    }

    private void FixedUpdate()
    {

        // 🔴 БЛОК ДВИЖЕНИЯ во время диалога
        if (GameState.IsDialogueOpen)
        {
            Debug.Log("BLOCK: Dialogue");
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (GameState.IsTransitioning)
        {
            Debug.Log("BLOCK: Transition");
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (GameState.IsMenuOpen)
        {
            Debug.Log("BLOCK: Menu");
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (GameState.IsInspecting)
        {
            Debug.Log("BLOCK: Inspect");
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 🟢 Движение
        Vector2 move = input.Player.Move.ReadValue<Vector2>().normalized;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (GameState.IsDialogueOpen || GameState.IsTransitioning || GameState.IsMenuOpen || GameState.IsInspecting)
            return;

        // Обновляем подсказку каждый кадр
        /*  UpdateHint(); */

        if (input.Player.Interact.WasPressedThisFrame())
            TryInteract();
    }
  
    void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);

        IInteractable found = null;
        //InteractionHint foundHint = null;

        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                found = interactable;
                //foundHint = hit.GetComponent<InteractionHint>();
                break;
            }
        }

        if (found != null)
            found.Interact();
    }

    private void OnDestroy()
    {
        Debug.Log($"{gameObject.name} → input уничтожен");
        input.Disable();
    }
    public bool IsAlive()
    {
        return _isAlive;
    }
     public bool IsRunning()
    {
        return _isRunning;
    }
     public Vector3 GetPlayerScreenPosition()
    {
        Vector3 playerScreenPosition = _mainCamera.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }
}