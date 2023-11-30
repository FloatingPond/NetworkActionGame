using Mirror;
using System;
using UnityEngine;

public class PlayerInputs : NetworkBehaviour
{
    public event Action<Vector2> OnMove, OnLook;
    public event Action<float> FreeLook;
    public event Action StopMove;

    public PlayerInputActions inputActions;
    public static PlayerInputs Instance { get; private set; }
    private void Awake()
    {
        #region Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        #endregion
        inputActions = new PlayerInputActions();

        #region Movement Events Subscription
        inputActions.Movement.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        inputActions.Movement.Move.canceled += ctx => StopMove?.Invoke();
        inputActions.Movement.Look.performed += ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>());
        #endregion

    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void FixedUpdate()
    {
        if (inputActions.Movement.Move.IsPressed())
        {
            OnMove?.Invoke(inputActions.Movement.Move.ReadValue<Vector2>());
        }
        else 
        {
            StopMove?.Invoke();
        }
        FreeLook?.Invoke(inputActions.Movement.FreeLook.ReadValue<float>());
    }
}
