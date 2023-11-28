using Mirror;

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
            UnityEngine.Debug.Log("Player Disconnected: " + conn.address);
        }
        base.OnServerDisconnect(conn);
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        GameManager.Instance.StartRoundLogic();
    }
}
