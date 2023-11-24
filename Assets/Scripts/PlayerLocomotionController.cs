using Mirror;
using UnityEngine;

public class PlayerLocomotionController : NetworkBehaviour
{
    private void Start()
    {
        if (isLocalPlayer && PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove -= OnMove;
            PlayerInputs.Instance.OnMove += OnMove;
        }
    }

    private void OnEnable()
    {
        if (PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove += OnMove;
        }
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnMove -= OnMove;
    }
    private void OnMove(Vector2 newVal)
    {
        Debug.Log(netIdentity + ": " + newVal);
    }
}
