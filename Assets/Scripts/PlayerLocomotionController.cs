using Mirror;
using UnityEngine;

public class PlayerLocomotionController : NetworkBehaviour
{
    [SerializeField] private CharacterController _CharacterController;

    #region Client-Server Code (Commands)

    [Command]
    private void RequestMovement(Vector2 newVal)
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
        float grav;

        if(_CharacterController.isGrounded)
        {
            grav = 0;
        }
        else
        {
            grav = -9.81f * Time.fixedDeltaTime;
        }

        _CharacterController.Move((new Vector3(moveVect.y * transform.forward.x, grav, moveVect.y * transform.forward.z)
                                   + new Vector3(moveVect.x * transform.right.x, _CharacterController.velocity.y * transform.forward.y, moveVect.x * transform.right.z)) 
                                   * (10 * Time.fixedDeltaTime));
    }

    #endregion

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnMove += RequestMovement;
        }

        transform.position = GameObject.Find("TestSpawnPoint").transform.position;
    }

    public override void OnStopLocalPlayer()
    {
        PlayerInputs.Instance.OnMove -= RequestMovement;

        base.OnStopLocalPlayer();
    }
}
