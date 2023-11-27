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
    [SerializeField]
    List<Player> hiders = new List<Player>();

    [Server]
    public void AddPlayerToList(Player newPlayer)
    {
        Debug.Log("Adding " + newPlayer.name + " to Player List");
        players.Add(newPlayer);

        if(players.Count == 1) // First player connected is a seeker
        {
            seekers.Add(newPlayer);
            newPlayer.UpdateTeam(Team.Seeker);
            newPlayer.UpdateColour(Color.red);
        }
        else if (players.Count / (5 * seekers.Count) > 1.2f) // If there aren't enough hiders to seekers (1 seeker for every 5 hiders)
        {
            // Add newly connected player to seekers;
            seekers.Add(newPlayer);
            newPlayer.UpdateTeam(Team.Seeker);
            newPlayer.UpdateColour(Color.red);
        }
        else
        {
            // Add new player to hiders.
            hiders.Add(newPlayer);
            newPlayer.UpdateTeam(Team.Hider);
            newPlayer.UpdateColour(Color.blue);
        }
    }

    [Server]
    public void RemovePlayerFromList(Player newPlayer)
    {
        Debug.Log("Removing " + newPlayer.name + " from Player List");
        players.Remove(newPlayer);
    }
}
