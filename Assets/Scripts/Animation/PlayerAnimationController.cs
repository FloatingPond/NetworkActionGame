using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;
    int horizontal;
    int vertical;
    [SerializeField] float moveAmount;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }
    private void OnEnable()
    {
        PlayerInputs.Instance.OnMove += RecieveInput;
        PlayerInputs.Instance.StopMove += StopMovement;
    }
    private void OnDisable()
    {
        PlayerInputs.Instance.OnMove -= RecieveInput;
        PlayerInputs.Instance.StopMove -= StopMovement;
    }

    private void StopMovement()
    {
        Debug.Log("Stop Movement");
        UpdateAnimatorValues(0, 0);
    }

    private void RecieveInput(Vector2 newMovement)
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(newMovement.x) + Mathf.Abs(newMovement.y));
        UpdateAnimatorValues(0, moveAmount);
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement)
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
        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }
}
