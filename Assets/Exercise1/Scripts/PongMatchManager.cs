using System.Collections;
using Mirror;
using UnityEngine;

public class PongMatchManager : NetworkBehaviour
{
    public static PongMatchManager Instance { get; private set; }

    [Header("Settings")]
    public GameObject ballPrefab;
    public int pointsToWin = 5;

    [SyncVar(hook = nameof(OnPlayer1ScoreChanged))] public int player1Score = 0;
    [SyncVar(hook = nameof(OnPlayer2ScoreChanged))] public int player2Score = 0;

    private GameObject currentBall;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [Server]
    public void StartRoundSequence()
    {
        StartCoroutine(RoundRoutine());
    }

    [Server]
    private IEnumerator RoundRoutine()
    {
        // 延迟5秒
        RpcUpdateRoundStatus("5秒后开始游戏");
        yield return new WaitForSeconds(5f);
        RpcUpdateRoundStatus("");

        SpawnBall();
    }

    [Server]
    private void SpawnBall()
    {
        if (currentBall != null)
        {
            NetworkServer.Destroy(currentBall);

            currentBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(currentBall);
        }
    }

    [Server]
    public void ScorePoint(int playerIndex)
    {
        if (playerIndex == 1)
        {
            player1Score++;
        }
        else
        {
            player2Score++;
        }

        RpcPlayRound("Score"); // 通知客户端播放得分音效

        if (player1Score >= pointsToWin)
        {
            EndMatch(1);
        }
        else if (player2Score >= pointsToWin)
        {
            EndMatch(2);
        }
        else
        {
            StartRoundSequence();
        }
    }

    [Server]
    private void EndMatch(int winnerIndex)
    {
        NetworkConnectionToClient p1Conn = NetworkServer.connections[0];
        NetworkConnectionToClient p2Conn = NetworkServer.connections[1];

        if (winnerIndex == 1)
        {
            TargetShowEndScreen(p1Conn, true);
            TargetShowEndScreen(p2Conn, false);
        }
        else
        {
            TargetShowEndScreen(p1Conn, false);
            TargetShowEndScreen(p2Conn, true);
        }
    }


    // --- Client RPCs (服务器调用，客户端执行) --- //
    [ClientRpc]
    private void RpcUpdateRoundStatus(string msg)
    {
        UIManager.Instance.UpdateStatus(msg);
    }

    [ClientRpc]
    public void RpcPlayRound(string soundType)
    {
        UIManager.Instance.PlaySound(soundType);
    }

    [TargetRpc]
    private void TargetShowEndScreen(NetworkConnection target, bool isWinner)
    {
        UIManager.Instance.ShowEndScreen(isWinner);

        // 需求：在结算画面断开连接
        if (isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    // --- SyncVar Hooks (数据变化时更新UI) --- //
    private void OnPlayer1ScoreChanged(int oldScore, int newScore)
    {
        UIManager.Instance.UpdateP1Score(newScore);
    }

    private void OnPlayer2ScoreChanged(int oldScore, int newScore)
    {
        UIManager.Instance.UpdateP2Score(newScore);
    }
}