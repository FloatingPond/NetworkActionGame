using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(ChangeColour))]
    Color teamColor;

    [SyncVar(hook = nameof(ChangeTeam))]
    public Team currentTeam;

    [Command]
    private void RequestAdditionToList()
    {
        GameManager.Instance.AddPlayerToList(this);
    }

    [Command]
    public void RequestRemovalFromList()
    {
        GameManager.Instance.RemovePlayerFromList(this);
    }

    [Client]
    private void ChangeTeam(Team _, Team newTeam)
    {
        currentTeam = newTeam;
    }

    [Client]
    private void ChangeColour(Color _, Color newCol)
    {
        gameObject.GetComponent<Renderer>().material.color = teamColor;
    }

    [Server]
    public void UpdateTeam(Team newTeam)
    {
        currentTeam = newTeam;
    }

    [Server]
    public void UpdateColour(Color newColor)
    {
        gameObject.GetComponent<Renderer>().material.color = newColor;
        teamColor = newColor;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        RequestAdditionToList();    
    }

    public override void OnStopLocalPlayer()
    {
        RequestRemovalFromList();

        base.OnStopLocalPlayer();
    }
}

public enum Team
{
    Seeker,
    Hider,
    Spectator
}