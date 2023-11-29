using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Client]
    public void UpdateRoundTimerText(string newVal)
    {
        roundTimerText.text = newVal;
    }

    [Client]
    public void UpdateRoundNumberText(string newVal)
    {
        roundNumberText.text = newVal;
    }

    [Client]
    public void UpdateWinningTeamText(string newVal)
    {
        ToggleWinScreen();
        winningTeamText.text = newVal;
    }

    [Client]
    public void ToggleWinScreen()
    {
        winningTeamPanel.SetActive(!winningTeamPanel.activeSelf);
    }

    [Client]
    public void UpdateSprintBarColour(Slider sprintBar)
    {
        if(sprintBar.value > .66f)
        {
            // green
            if(sprintBar.fillRect.TryGetComponent(out Image img))
            {
                img.color = Color.green;
            }
        }
        else if (sprintBar.value > .33f)
        {
            // yellow
            if (sprintBar.fillRect.TryGetComponent(out Image img))
            {
                img.color = Color.yellow;
            }
        }
        else
        {
            // red
            if (sprintBar.fillRect.TryGetComponent(out Image img))
            {
                img.color = Color.red;
            }
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
