using Mirror;
using System.Diagnostics;

public class FP_NetworkManager : NetworkManager
{
    [Server]
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        for (int i = 0; i < conn.owned.Count; i++)
        {
            if (conn.identity.TryGetComponent(out Player player))
            {
                GameManager.Instance.RemovePlayerFromAllLists(player);
            }
            UnityEngine.Debug.Log("Player disconnected: " + conn.address);
        }
        base.OnServerDisconnect(conn);
    }

    [Server]
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        UnityEngine.Debug.Log("Player " + conn.address + " connected.");
        GameManager.Instance.CheckRoundStartReqs();
    }
}
