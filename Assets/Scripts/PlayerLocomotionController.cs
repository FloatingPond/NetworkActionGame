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
        
    }

    [Client]
    private void StartClient()
    {
        if (PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove -= TryMove;
            PlayerInputs.Instance.OnMove += TryMove;
        }
    }

    private void OnEnable()
    {
        if (isLocalPlayer && PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove += TryMove;
        }
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.OnMove -= TryMove;
    }

    [Command]
    private void OnMove(Vector2 newVal)
    {
        Debug.Log(netIdentity + ": " + newVal);
    }
    private void TryMove(Vector2 newVal)
    {
        Debug.Log("Trying to move");
        OnMove(newVal);
    }
}
