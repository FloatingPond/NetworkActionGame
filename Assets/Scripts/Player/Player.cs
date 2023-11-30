using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(ChangeColour))]
    Color teamColor;

    [SyncVar(hook = nameof(ChangeTeam))]
    public Team currentTeam;

    [SyncVar(hook = nameof(ChangeName))]
    public string playerName;

    [Command]
    private void RequestAdditionToList()
    {
        GameManager.Instance.AddPlayerToList(this);
    }

    [Command]
    public void RequestRemovalFromList()
    {
        GameManager.Instance.RemovePlayerFromAllLists(this);
    }

    [Command]
    public void RequestPlayerCollision(Team newTeam)
    {
        GameManager.Instance.SwapTeam(this, newTeam);
        UpdateTeam(newTeam);
        GameManager.Instance.CheckWinCondition();
    }

    [Command]
    public void RequestNameChange(string newName)
    {
        UpdatePlayerName(newName);
    }

    [Client]
    private void ChangeTeam(Team _, Team newTeam)
    {
        currentTeam = newTeam;
        Debug.Log("Team has been changed.");
        // Update Team UI
        UIManager.Instance.UpdateTeamTextRPC(connectionToClient, currentTeam.ToString(), (int)currentTeam);
    }
    
    [Client]
    private void ChangeName(string _, string newName)
    {
        name = playerName;
       // UIManager.Instance.UpdatePlayerName(newName, (int)currentTeam);
    }

    [Client]
    private void ChangeColour(Color _, Color newCol)
    {
       if(gameObject.TryGetComponent(out Renderer renderer))
            renderer.material.color = teamColor;
    }

    [Server]
    public void UpdateTeam(Team newTeam)
    {
        currentTeam = newTeam;
        switch (currentTeam)
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
        if(gameObject.TryGetComponent(out Renderer renderer))
        {
            renderer.material.color = newColor;
        }
        teamColor = newColor;
    }

    [Server]
    public void UpdatePlayerName(string newName)
    {
        playerName = newName;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        
        RequestAdditionToList();
        RequestNameChange(Steamworks.SteamFriends.GetPersonaName());

        //Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnStopLocalPlayer()
    {
        //Cursor.lockState = CursorLockMode.Confined;

        base.OnStopLocalPlayer();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLocalPlayer || !GameManager.Instance.roundLive) return;

        if (collision.collider.TryGetComponent(out Player enemy))
        {
            if (enemy.currentTeam == currentTeam) return; // The player we collided with was on our team.

            if (currentTeam == Team.Hider)
            {
                RequestPlayerCollision(Team.Seeker);
            }
        }
    }
}

public enum Team
{
    Seeker,
    Hider,
    Spectator,
    Null
}