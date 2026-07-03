using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions _playerInputActions;


    

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError(" GameInput is not yet implemented!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
     

    }


    public Vector2 GetMovementVector()
    {
        if (_playerInputActions == null) return Vector2.zero;

        return _playerInputActions.Player.Move.ReadValue<Vector2>();
    }
    public static Vector2 GetMousePosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }
    private void OnDestroy()
    {
        _playerInputActions?.Disable();
    }

    public void DisableMovement()
    {
        _playerInputActions.Disable();
    }
}