using Mirror;
using UnityEngine;

public class PlayerLocomotionController : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private Vector3 moveDirection;
    #region Client-Server Code (Commands)

    [Command]
    private void RequestOnMove(Vector2 newVal)
    {
        UpdatePlayerMovement(newVal);
    }

    #endregion

    #region Client-Side Code

    #endregion

    #region Server-Side Code

    [Server]
    private void UpdatePlayerMovement(Vector2 moveVect)
    {
        moveDirection = transform.forward * moveVect.y;
        moveDirection += transform.right * moveVect.x;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection *= movementSpeed;

        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
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
            PlayerInputs.Instance.OnMove += RequestOnMove;
        }

        transform.position = GameObject.Find("TestSpawnPoint").transform.position;
    }

    public override void OnStopLocalPlayer()
    {
        PlayerInputs.Instance.OnMove -= RequestOnMove;

        base.OnStopLocalPlayer();
    }
}
