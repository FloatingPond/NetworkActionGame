using Mirror;
using UnityEngine;

public class PlayerAnimationController : NetworkBehaviour
{
    [SerializeField] PlayerLocomotionController playerLoco;

    [SerializeField] Animator animator;
    int horizontal;
    int vertical;
    public float moveAmount;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        PlayerInputs.Instance.StopMove += RequestStopAnims;
    }

    public override void OnStopLocalPlayer()
    {
        PlayerInputs.Instance.StopMove -= RequestStopAnims;
        base.OnStopLocalPlayer();
    }

    [Client]
    private void RequestStopAnims()
    {
        StopMovement();
        playerLoco.UpdatePlayerMovement(Vector2.zero);
    }

    [Command]
    private void StopMovement()
    {
        Debug.Log("Stop Movement");
        UpdateAnimatorValues(0, 0, false);
    }

    [Server]
    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontal;
        float snappedVertical;
        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1;
        }
        else
        { 
            snappedHorizontal = 0;
        }
        #endregion
        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        #endregion

        if (isSprinting) 
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2;
        }
        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.fixedDeltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.fixedDeltaTime);
    }
}
