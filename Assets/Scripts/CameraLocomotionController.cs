using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class CameraLocomotionController : NetworkBehaviour
{
    [SerializeField]
    bool freeLook = false;

    [Client]
    private void OnEnable()
    {
        if (isLocalPlayer && PlayerInputs.Instance != null)
        {
            PlayerInputs.Instance.OnLook += OnLook;
            PlayerInputs.Instance.FreeLook += FreeLook;
        }
    }

    [Client]
    private void OnDisable()
    {
        PlayerInputs.Instance.OnLook -= OnLook;
        PlayerInputs.Instance.FreeLook -= FreeLook;
    }

    [Client]
    private void FreeLook(float obj)
    {
        if (obj == 1)
            freeLook = true;
        else
        {
            ResetCamera();
            freeLook = false;
        }
    }

    [Client]
    private void OnLook(Vector2 lookVect)
    {
        if (freeLook)
            RotateCamera(lookVect);
        else
            RequestPlayerRotation(lookVect);
    }

    [Client]
    private void RotateCamera(Vector2 lookVect)
    {
        Camera.main.transform.RotateAround(transform.position, Vector3.up, lookVect.x * 20 * Time.fixedDeltaTime);
    }

    [Client]
    private void ResetCamera()
    {
        Camera.main.transform.localRotation = Quaternion.identity;
        Camera.main.transform.localPosition = new Vector3(0, 1, -5);
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

    public override void OnStopClient()
    {
        if (isLocalPlayer)
        {
            PlayerInputs.Instance.OnLook -= OnLook;
            PlayerInputs.Instance.FreeLook -= FreeLook;

            Camera.main.transform.SetParent(null);
            ResetCamera();
        }

        base.OnStopClient();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            if (PlayerInputs.Instance != null)
            {
                PlayerInputs.Instance.OnLook += OnLook;
                PlayerInputs.Instance.FreeLook += FreeLook;
            }

            Camera.main.transform.SetParent(gameObject.transform);
            ResetCamera();
        }
    }
}
