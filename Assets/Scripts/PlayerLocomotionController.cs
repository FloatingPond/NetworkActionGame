using Mirror;
using UnityEngine;

public class PlayerLocomotionController : NetworkBehaviour
{
    [SyncVar(hook = nameof(UpdateColour))]
    Color newColor;

    private void Start()
    {
        if (isServerOnly)
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
        if (isLocalPlayer && PlayerInputs.Instance != null)
        {
            Debug.Log("Client subscribing to inputs...");
            PlayerInputs.Instance.OnMove -= OnMove;
            PlayerInputs.Instance.OnMove += OnMove;
        }
    }

    [Client]
    private void OnEnable()
    {
        if (isLocalPlayer && PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove += OnMove;
        }
    }

    [Client]
    private void OnDisable()
    {
        PlayerInputs.Instance.OnMove -= OnMove;
    }

    [Command]
    private void OnMove(Vector2 newVal)
    {
        SendMessageRPC(netIdentity + ": " + newVal);
        ChangeColour();
    }

    [ClientRpc]
    private void SendMessageRPC(string newMessage)
    {
        Debug.Log(newMessage);
    }

    [Server]
    private void ChangeColour()
    {
        newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        gameObject.GetComponent<Renderer>().material.color = newColor;
    }

    [Client]
    private void UpdateColour(Color _, Color newCol)
    {
        gameObject.GetComponent<Renderer>().material.color = newColor;
    }
}
