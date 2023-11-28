using Mirror;
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
}
