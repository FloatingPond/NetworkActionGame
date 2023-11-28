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

    [Command]
    public void RequestPlayerCollision(Team newTeam)
    {
        UpdateTeam(newTeam);
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
        switch(currentTeam)
        {
            case Team.Seeker:
                UpdateColour(GameManager.Instance.GetSeekerTeamColor);
                break;
            case Team.Hider:
                UpdateColour(GameManager.Instance.GetHiderTeamColor);
                break;
        }
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
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnStopLocalPlayer()
    {
        RequestRemovalFromList();
        Cursor.lockState = CursorLockMode.Confined;

        base.OnStopLocalPlayer();
    }

    [Client]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player enemy))
        {
            if (enemy.currentTeam == currentTeam) return; // The player we collided with was on our team.
            
            if(currentTeam == Team.Hider)
                RequestPlayerCollision(Team.Seeker);
        }
    }
}

public enum Team
{
    Seeker,
    Hider,
    Spectator
}