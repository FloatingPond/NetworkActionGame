using System;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    public event Action<Vector2> OnMove, OnLook;

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
}