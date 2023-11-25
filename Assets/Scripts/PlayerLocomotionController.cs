using Mirror;
using UnityEngine;

public class PlayerLocomotionController : NetworkBehaviour
{
    [SerializeField] private CharacterController _CharacterController;

    [SyncVar(hook = nameof(UpdateColour))]
    Color newColor;

    [SerializeField]
    bool freeLook = false;

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
        RequestMovement(newVal);
    }

    [Command]
    private void RotatePlayerModel(Vector2 newRotation)
    {
        RequestPlayerRotation(newRotation);
    }

    [Command]
    private void RequestChangeColour()
    {
        ChangeColour();
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
            PlayerInputs.Instance.OnLook -= OnLook;
            PlayerInputs.Instance.OnLook += OnLook;
            PlayerInputs.Instance.freeLook -= FreeLook;
            PlayerInputs.Instance.freeLook += FreeLook;

            Camera.main.transform.SetParent(gameObject.transform);
            Camera.main.transform.localPosition = new Vector3(0, 1, -5);
        }
    }

    [Client]
    private void FreeLook(float obj)
    {
        if (obj == 1)
            freeLook = true;
        else
        {
            ResetCameraRotation();
            freeLook = false;
        }
    }

    [Client]
    private void OnLook(Vector2 lookVect)
    {
        if (freeLook)
            RotateCamera(lookVect);
        else
            RotatePlayerModel(lookVect);
    }

    [Client]
    private void RotateCamera(Vector2 lookVect)
    {
        Camera.main.transform.RotateAround(transform.position, Vector3.up, lookVect.x * 20 * Time.fixedDeltaTime);
    }

    [Client]
    private void ResetCameraRotation()
    {
        Camera.main.transform.localRotation = Quaternion.identity;
        Camera.main.transform.localPosition = new Vector3(0,1,-5);
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

    [Server]
    private void RequestPlayerRotation(Vector2 newRotation)
    {
        transform.Rotate(new Vector3(0, newRotation.x, 0) * 20 * Time.fixedDeltaTime);
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

        transform.position = GameObject.Find("TestSpawnPoint").transform.position;
        RequestChangeColour();
    }
}
