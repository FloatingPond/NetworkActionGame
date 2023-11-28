using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }
    #endregion

    [SerializeField] GameObject SeekerSpawnBox;

    [SerializeField]
    List<Player> players = new List<Player>();

    [SerializeField]
    List<Player> seekers = new List<Player>();
    private Color seekerTeamColor = Color.red;
    public Color GetSeekerTeamColor => seekerTeamColor;

    [SerializeField]
    List<Player> hiders = new List<Player>();
    private Color hiderTeamColor = Color.blue;
    public Color GetHiderTeamColor => hiderTeamColor;

    public event Action RoundStart, RoundEnd, RoundIntermission, RoundInProgress, SeekersReleased;

    [SerializeField]
    float timeRemaining;

    private void Start()
    {
        RoundStart += UpdateStartRound;
        RoundEnd += UpdateEndRound;
        RoundInProgress += UpdateRoundProgress;
        RoundIntermission += UpdateRoundIntermission;
        SeekersReleased += UpdateSeekersRelease;

        StartRoundLogic();
    }

    [Server]
    public void StartRoundLogic()
    {
        StartCoroutine(RoundLogic());
    }

    IEnumerator RoundLogic()
    {
        while(players.Count < 2)
        {
            yield return new WaitForSeconds(1.0f);
        }

        RoundStart?.Invoke();
    }

    [Server]
    public void AddPlayerToList(Player newPlayer)
    {
        Debug.Log("Adding " + newPlayer.name + " to Player List");
        players.Add(newPlayer);

        if(players.Count == 1) // First player connected is a seeker
        {
            seekers.Add(newPlayer);
            newPlayer.UpdateTeam(Team.Seeker);
            newPlayer.UpdateColour(seekerTeamColor);
        }
        else if (players.Count / (5 * seekers.Count) > 1.2f) // If there aren't enough hiders to seekers (1 seeker for every 5 hiders)
        {
            // Add newly connected player to seekers;
            seekers.Add(newPlayer);
            newPlayer.UpdateTeam(Team.Seeker);
            newPlayer.UpdateColour(seekerTeamColor);
        }
        else
        {
            // Add new player to hiders.
            hiders.Add(newPlayer);
            newPlayer.UpdateTeam(Team.Hider);
            newPlayer.UpdateColour(hiderTeamColor);
        }
    }

    [Server]
    public void RemovePlayerFromAllLists(Player newPlayer)
    {
        Debug.Log("Removing " + newPlayer.name + " from Player List");
        players.Remove(newPlayer);
        switch (newPlayer.currentTeam)
        {
            case Team.Hider:
                hiders.Remove(newPlayer);
                break;
            case Team.Seeker:
                seekers.Remove(newPlayer);
                break;
        }
    }

    [Server]
    public void SwapTeam(Player currentPlayer, Team newTeam)
    {
        switch (currentPlayer.currentTeam)
        {
            case Team.Hider:
                hiders.Remove(currentPlayer);
                break;
            case Team.Seeker:
                seekers.Remove(currentPlayer);
                break;
        }        
        
        switch (newTeam)
        {
            case Team.Hider:
                hiders.Add(currentPlayer);
                break;
            case Team.Seeker:
                seekers.Add(currentPlayer);
                break;
        }
    }

    [Server]
    public void UpdateStartRound()
    {
        // Move Players to round start locations based on team
        foreach(Player seeker in seekers)
        {
            seeker.transform.position = SeekerSpawnBox.transform.position;
        }

        float roundLength = 10f;
        timeRemaining = roundLength;
        while (timeRemaining > 0)
        {
            // Update UI with time remaining
            timeRemaining -= Time.deltaTime;
        }

        RoundInProgress?.Invoke();
        // Invoke RoundInProgress and SeekersReleased once timer has finished
    }

    [Server]
    public void UpdateSeekersRelease()
    {
        // Count down 20 seconds to seeker release
        float seekerReleaseTimer = 20f;
        while (seekerReleaseTimer > 0)
        {
            // Update UI with time remaining
            seekerReleaseTimer -= Time.deltaTime;
        }

        // Open Seeker Door here
    }

    [Server]
    public void UpdateRoundProgress()
    {
        SeekersReleased?.Invoke();

        float roundLength = 10f;
        timeRemaining = roundLength * 60;
        while (timeRemaining > 0)
        {
            // Update UI with time remaining
            timeRemaining -= Time.deltaTime;

            if (hiders.Count == 0) break;
        }

        // Invoke RoundEnd
        RoundEnd?.Invoke();
    }

    [Server]
    public void UpdateEndRound()
    {
        if(timeRemaining <= 0) 
        {
            // Hiders Win
        }
        else
        {
            // Seekers Win
        }

        // Count down win screen timer... (maybe 5-10seconds)
        float roundLength = 10f;
        timeRemaining = roundLength;
        while (timeRemaining > 0)
        {
            // Update UI with time remaining
            timeRemaining -= Time.deltaTime;
        }

        RoundIntermission?.Invoke();
        // Invoke RoundIntermission
    }

    [Server]
    public void UpdateRoundIntermission()
    {
        // Start countdown to next round start
        float roundLength = 2f;
        timeRemaining = roundLength * 60;
        while (timeRemaining > 0)
        {
            // Update UI with time remaining
            timeRemaining -= Time.deltaTime;
        }

        RoundStart?.Invoke();

    }
}
