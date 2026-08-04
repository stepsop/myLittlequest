using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerVisual : MonoBehaviour
{
    private static readonly int Running = Animator.StringToHash(IS_RUNNING);
    private const string IS_RUNNING = "IsRunning";

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        if (_animator == null)
            Debug.LogError("Animator component is missing!", this);
    }

    private void OnEnable()
    {
       
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_animator == null) return;

   
        _animator.Rebind();
        _animator.Update(0f);
    }

    private void Update()
    {
        if (_animator == null) return; // защита от NRE, если Animator не найден
        if (PlayerMovement.Instance == null) return;

        _animator.SetBool(Running, PlayerMovement.Instance.IsRunning());

            AdjustPlayerFacingDirection();
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = GameInput.GetMousePosition();
        Vector3 playerPosition = PlayerMovement.Instance.GetPlayerScreenPosition();
        _spriteRenderer.flipX = mousePos.x < playerPosition.x;
    }
}