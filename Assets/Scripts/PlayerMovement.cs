using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private PlayerInputActions input;
    /* private InteractionHint currentHint; */

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = new PlayerInputActions();
        input.Enable();
    }

    private void FixedUpdate()
    {
        // 🔴 БЛОК ДВИЖЕНИЯ во время диалога
        if (GameState.IsDialogueOpen || GameState.IsTransitioning|| GameState.IsMenuOpen)//|| GameState.IsInventoryOpen)
        {
            rb.linearVelocity = Vector2.zero; // гарантированно останавливаем игрока
            return;
        }

        // 🟢 Движение
        Vector2 move = input.Player.Move.ReadValue<Vector2>().normalized;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (GameState.IsDialogueOpen || GameState.IsTransitioning|| GameState.IsMenuOpen)
            return;

        // Обновляем подсказку каждый кадр
       /*  UpdateHint(); */

        if (input.Player.Interact.WasPressedThisFrame())
            TryInteract();
    }
   /*  void UpdateHint()
    { */
        /* Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);

        InteractionHint nearestHint = null;

        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                nearestHint = hit.GetComponent<InteractionHint>();
                break;
            }
        } */

        // Если подсказка изменилась — скрываем старую показываем новую
        /* if (nearestHint != currentHint)
        {
            if (currentHint != null) currentHint.Hide();
            currentHint = nearestHint;
            if (currentHint != null) currentHint.Show();
        } */
   /*  } */

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
        input.Disable();
    }
}