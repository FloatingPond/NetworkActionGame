using Mirror;
using UnityEngine;

public class CameraLocomotionController : NetworkBehaviour
{
    [SerializeField]
    bool freeLook = false;

    [SerializeField]
    GameObject CameraPitch;

    [SerializeField]
    GameObject CameraYaw;

    //[Client]
    //private void OnEnable()
    //{
    //    if (isLocalPlayer && PlayerInputs.Instance != null)
    //    {
    //        PlayerInputs.Instance.OnLook += OnLook;
    //        PlayerInputs.Instance.FreeLook += FreeLook;
    //    }
    //}

    //[Client]
    //private void OnDisable()
    //{
    //    PlayerInputs.Instance.OnLook -= OnLook;
    //    PlayerInputs.Instance.FreeLook -= FreeLook;
    //}

    [Client]
    private void FreeLook(float obj)
    {
        if (obj == 1)
            freeLook = true;
        else
        {
            if (freeLook)
            {
                ResetCamera();
                freeLook = false;
            }
        }
    }

    public float newAngle;
    public float newRotation;
    public float currentRotation;
    [Client]
    private void OnLook(Vector2 lookVect)
    {
        newAngle = -lookVect.y * 20 * Time.fixedDeltaTime;
        CameraPitch.transform.Rotate(newAngle, 0, 0);
        newRotation = currentRotation + newAngle;
        SetCurrentRotation(newRotation);

        RotateCamera(lookVect);
    }

    [Client]
    private void SetCurrentRotation(float rot)
    {
        currentRotation = Mathf.Clamp(rot, -45f, 45f);
        CameraPitch.transform.localRotation = Quaternion.Euler(rot, 0, 0);
    }

    [Client]
    private void RotateCamera(Vector2 lookVect)
    {
        if (freeLook)      
            CameraYaw.transform.Rotate(new Vector3(0, lookVect.x, 0) * 20 * Time.fixedDeltaTime);
        else
        {
            RequestPlayerRotation(lookVect);
        }
    }

    [Client]
    private void ResetCamera()
    {
        CameraPitch.transform.localRotation = Quaternion.identity;
        CameraYaw.transform.localRotation = Quaternion.identity;
        newAngle = 0;
        newRotation = 0;
        currentRotation = 0;
    }

    [Command]
    private void RequestPlayerRotation(Vector2 newRotation)
    {
        UpdatePlayerRotation(newRotation);
    }

    [Server]
    private void UpdatePlayerRotation(Vector2 newRotation)
    {
        transform.Rotate(new Vector3(0, newRotation.x, 0) * 20 * Time.fixedDeltaTime);
    }

    public override void OnStopLocalPlayer()
    {
        PlayerInputs.Instance.OnLook -= OnLook;
        PlayerInputs.Instance.FreeLook -= FreeLook;

        Camera.main.transform.SetParent(null);
        Camera.main.transform.localPosition = new Vector3(0, 1, -5);
        Camera.main.transform.localRotation = Quaternion.identity;

        base.OnStopLocalPlayer();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnLook += OnLook;
            PlayerInputs.Instance.FreeLook += FreeLook;
        }

        Camera.main.transform.SetParent(CameraPitch.transform);
        Camera.main.transform.localPosition = new Vector3(0, 1, -5);
        Camera.main.transform.localRotation = Quaternion.identity;
        ResetCamera();
    }
}
