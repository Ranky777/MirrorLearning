using Mirror;
using UnityEngine;

public class PlayerLogic : NetworkBehaviour
{
    public float moveSpeed = 5f;

    [SyncVar(hook = nameof(OnScoreChanged))]
    public int score = 10;

    private void Update()
    {
        if (isLocalPlayer)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;

            transform.Translate(movement);
        }
    }

    private void OnScoreChanged(int oldScore, int newScore)
    {
        float newSize = 1f + (newScore - 10) * 0.1f;
        transform.localScale = new Vector3(newSize, newSize, newSize);
    }

    // 碰撞检测（必须只在服务器上执行，防止客户端作弊）
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        // 1. 吃到食物
        if (other.CompareTag("Food"))
        {
            score += 1;
            NetworkServer.Destroy(other.gameObject); // 销毁食物
        }
        // 2. 碰到其它玩家
        else if (other.CompareTag("Player"))
        {
            PlayerLogic otherPlayer = other.GetComponent<PlayerLogic>();

            // 如果我的分数大于对方，我吃掉对方
            if (otherPlayer != null && score > otherPlayer.score)
            {
                score += otherPlayer.score; // 夺取对方分数
                otherPlayer.RespawnOnServer(); // 让对方重生
            }
        }
    }

    // 在服务器上调用，处理重置逻辑，并通知客户端传送
    [Server]
    public void RespawnOnServer()
    {
        score = 10;
        Vector3 randomSpawnPos = new Vector3(Random.Range(-15f, 15f), 1f, Random.Range(-15f, 15f));
        RpcTeleport(randomSpawnPos); // 通知客户端传送
    }

    [ClientRpc]
    private void RpcTeleport(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
