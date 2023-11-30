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

    #endregion

    #region Client-Side Code

    [Client]
    private void HandleSprinting()
    {
        //if (PlayerInputs.Instance.sprint && moveAmount > 0.5f)
        if (PlayerInputs.Instance.inputActions.Movement.Sprint.IsPressed() && moveAmount > 0.5f)
        {
            UpdateSprint(true);
        }
        else
        {
            UpdateSprint(false);
        }
    }

    [Command]
    private void UpdateSprint(bool _)
    {
        PlayerInputs.Instance.sprint = _;
    }

    [Client]
    public void ChangeAnimatorValues(Vector2 newVal)
    {
        UpdatePlayerMovement(newVal);
    }

    #endregion

    #region Server-Side Code

    public float moveAmount;
    public float currentMoveSpeed = 0;
    [Command]
    private void UpdatePlayerMovement(Vector2 moveVect)
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(moveVect.x) + Mathf.Abs(moveVect.y));
        playerAnimationController.UpdateAnimatorValues(0, moveAmount, PlayerInputs.Instance.sprint);

        moveDirection = transform.forward * moveVect.y;
        moveDirection += transform.right * moveVect.x;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (PlayerInputs.Instance.sprint)
        {
            moveDirection *= sprintSpeed;
            currentMoveSpeed = sprintSpeed;
        }
        else
        {
            if (moveAmount >= 0.5f)
            {
                moveDirection *= runSpeed;
                currentMoveSpeed = runSpeed;
            }
            else
            {
                moveDirection *= walkSpeed;
                currentMoveSpeed = walkSpeed;
            }
        }
        
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }

    #endregion

    private void Update()
    {
        HandleSprinting();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove += ChangeAnimatorValues;
        }

        transform.position = GameObject.Find("TestSpawnPoint").transform.position;
    }

    public override void OnStopLocalPlayer()
    {
        PlayerInputs.Instance.OnMove -= ChangeAnimatorValues;

        base.OnStopLocalPlayer();
    }
}
