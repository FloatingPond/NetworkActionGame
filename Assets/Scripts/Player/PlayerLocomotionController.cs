using Mirror;
using UnityEngine;
[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerLocomotionController : NetworkBehaviour
{
    [SerializeField] private PlayerAnimationController playerAnimationController;
    [SerializeField] private Rigidbody rb;
    [SerializeField, Header("Movement Speeds")] private float walkSpeed = 1.5f, runSpeed = 7f, sprintSpeed = 12f;
    [SerializeField] bool isSprinting = false;
    private Vector3 moveDirection;
    #region Client-Server Code (Commands)
    private void Update()
    {
        HandleSprinting();
    }
    [Command]
    private void RequestOnMove(Vector2 newVal)
    {
        UpdatePlayerMovement(newVal);
    }

    #endregion

    #region Client-Side Code

    [Client]
    private void HandleSprinting()
    {
        if (PlayerInputs.Instance.sprint && playerAnimationController.moveAmount > 0.5f)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    #endregion

    #region Server-Side Code

    [Server]
    private void UpdatePlayerMovement(Vector2 moveVect)
    {
        moveDirection = transform.forward * moveVect.y;
        moveDirection += transform.right * moveVect.x;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (isSprinting)
        {
            moveDirection *= sprintSpeed;
        }
        else
        {
            if (playerAnimationController.moveAmount >= 0.5f)
            {
                moveDirection *= runSpeed;
            }
            else
            {
                moveDirection *= walkSpeed;
            }
        }

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
