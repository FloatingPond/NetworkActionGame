using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private TextMeshProUGUI roundTimerText;
    [SerializeField] private TextMeshProUGUI roundNumberText;
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.UpdateRoundNumberEvent += UpdateRoundNumberText;
        GameManager.Instance.UpdateRoundTimerEvent += UpdateRoundTimerText;
    }
    public void UpdateRoundTimerText(string newVal)
    {
        roundTimerText.text = newVal;
    }

    public void UpdateRoundNumberText(string newVal)
    {
        roundNumberText.text = newVal;
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
