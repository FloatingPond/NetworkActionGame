using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private TextMeshProUGUI roundTimerText;
    [SerializeField] private TextMeshProUGUI roundNumberText;
    [SerializeField] private TextMeshProUGUI winningTeamText;
    [SerializeField] private GameObject winningTeamPanel;

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
        GameManager.Instance.UpdateWinningTeamEvent += UpdateWinningTeamText;

        winningTeamPanel.SetActive(false);
    }

    public void UpdateRoundTimerText(string newVal)
    {
        roundTimerText.text = newVal;
    }

    public void UpdateRoundNumberText(string newVal)
    {
        roundNumberText.text = newVal;
    }

    public void UpdateWinningTeamText(string newVal)
    {
        ToggleWinScreen();
        winningTeamText.text = newVal;
    }

    public void ToggleWinScreen()
    {
        winningTeamPanel.SetActive(!winningTeamPanel.activeSelf);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
