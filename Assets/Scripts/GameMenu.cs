using Mirror;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    public void DisconnectAndExit()
    {
        // 如果是主机（既是服务器又是客户端）
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
    }
}