using Mirror;
using UnityEngine;

public class PlayerLocomotionController : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private Vector3 moveDirection;
    #region Client-Server Code (Commands)

    [Command]
    private void OnMove(Vector2 newVal)
    {
        UpdatePlayerMovement(newVal, Camera.main.transform);
    }

    #endregion

    #region Client-Side Code

    #endregion

    #region Server-Side Code

    [Server]
    private void UpdatePlayerMovement(Vector2 moveVect, Transform camera)
    {
        moveDirection = camera.forward * moveVect.y;
        moveDirection += camera.right * moveVect.x;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection *= movementSpeed;

        rb.velocity = moveDirection;
    }

    #endregion

    [ClientRpc]
    private void SendMessageRPC(string newMessage)
    {
        Debug.Log(newMessage);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove += OnMove;
        }

        transform.position = GameObject.Find("TestSpawnPoint").transform.position;
    }

    public override void OnStopLocalPlayer()
    {
        PlayerInputs.Instance.OnMove -= OnMove;

        base.OnStopLocalPlayer();
    }
}
