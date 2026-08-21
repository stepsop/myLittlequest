using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movingSpeed = 5f;

    private Rigidbody2D rb;
    private PlayerInputActions input;
    public static PlayerMovement Instance { get; private set; }
    private Camera _mainCamera;


    private bool _isRunning;



    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        Debug.Log($"{gameObject.name} → input создан");
        input = new PlayerInputActions();
        input.Player.Enable();
    }

    private void FixedUpdate()
    {
        // Если движение заблокировано (диалог/переход/меню/осмотр) — стоим на месте
        if (IsMovementBlocked())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        HandleMovement();
    }

    // Двигает игрока и обновляет состояние "бежит/стоит" для аниматора
    private void HandleMovement()
    {
        Vector2 move = input.Player.Move.ReadValue<Vector2>();


        _isRunning = move.sqrMagnitude > 0.01f;

        rb.MovePosition(rb.position + move.normalized * movingSpeed * Time.fixedDeltaTime);
    }

    // Проверяет, разрешено ли сейчас движение
    private bool IsMovementBlocked()
    {
        return GameState.IsDialogueOpen
            || GameState.IsTransitioning
            || GameState.IsMenuOpen
            || GameState.IsInspecting;
    }

    private void Update()
    {
        if (IsMovementBlocked())
        {
            Debug.Log("[Interact] blocked by GameState");
            return;
        }

        if (input.Player.Interact.WasPressedThisFrame())
        {
            Debug.Log("[Interact] Interact action pressed");
            MouseInteractDetector.Instance?.TryInteractUnderCursor();
        }
    }

    private void OnDestroy()
    {
        input?.Disable();

        if (Instance == this)
            Instance = null;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }
    public Vector3 GetPlayerScreenPosition()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
        Vector3 playerScreenPosition = _mainCamera.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }
}