using Mirror;
using UnityEngine;

public class PlayerLocomotionController : NetworkBehaviour
{
    [SerializeField] private CharacterController _CharacterController;

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



    #region Client-Server Code (Commands)

    [Command]
    private void OnMove(Vector2 newVal)
    {
        if (newVal == Vector2.zero) return;
        
        SendMessageRPC(netIdentity + ": " + newVal);
        //ChangeColour();
        RequestMovement(newVal);
    }

    #endregion

    #region Client-Side Code
    [Client]
    private void StartClient()
    {
        Debug.Log("Starting Client...");
        if (isLocalPlayer && PlayerInputs.Instance != null)
        {
            Debug.Log("Client subscribing to inputs...");
            PlayerInputs.Instance.OnMove -= OnMove;
            PlayerInputs.Instance.OnMove += OnMove;

            Camera.main.transform.SetParent(gameObject.transform);
            Camera.main.transform.localPosition = new Vector3(0, 1, -5);
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
    
    [Client]
    private void UpdateColour(Color _, Color newCol)
    {
        gameObject.GetComponent<Renderer>().material.color = newColor;
    }
    #endregion

    #region Server-Side Code
    [Server]
    private void StartServer()
    {
        Debug.Log("Starting Server...");
    }

    [Server]
    private void ChangeColour()
    {
        newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        gameObject.GetComponent<Renderer>().material.color = newColor;
    }

    [Server]
    private void RequestMovement(Vector2 moveVect)
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
                + new Vector3(moveVect.x * transform.right.x,
                                _CharacterController.velocity.y * transform.forward.y,
                                moveVect.x * transform.right.z)) * (2 * Time.fixedDeltaTime));
    }
    #endregion

    [ClientRpc]
    private void SendMessageRPC(string newMessage)
    {
        Debug.Log(newMessage);
    }

    public override void OnStopClient()
    {
        Camera.main.transform.SetParent(null);

        base.OnStopClient();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }
}
