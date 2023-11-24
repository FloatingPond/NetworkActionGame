using Mirror;
using UnityEngine;

public class PlayerLocomotionController : NetworkBehaviour
{
    private void Start()
    {
        if (isServer)
        {
            StartServer();
        }
        else
        {
            StartClient();
        }
    }

    [Server]
    private void StartServer()
    {
        Debug.Log("Starting Server...");
    }

    [Client]
    private void StartClient()
    {
        Debug.Log("Starting Client...");
        if (PlayerInputs.Instance != null)
        {
            Debug.Log("Client subscribing to inputs...");
            PlayerInputs.Instance.OnMove -= OnMove;
            PlayerInputs.Instance.OnMove += OnMove;
        }
    }

    private void OnEnable()
    {
        if (isLocalPlayer && PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove += OnMove;
        }
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnMove -= OnMove;
    }

    [Command]
    private void OnMove(Vector2 newVal)
    {
        Debug.Log(netIdentity + ": " + newVal);
    }
}
