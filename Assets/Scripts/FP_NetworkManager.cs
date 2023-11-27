using Mirror;
using System.Diagnostics;
using UnityEngine;
public class FP_NetworkManager : NetworkManager
{
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        for (int i = 0; i < conn.owned.Count; i++)
        {
            conn.identity.GetComponent<Player>().RequestRemovalFromList();
            UnityEngine.Debug.Log("Player Disconnected: " + conn.address);
        }
        base.OnServerDisconnect(conn);
    }
    
}
