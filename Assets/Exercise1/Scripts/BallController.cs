using Mirror;
using UnityEngine;

public class BallController : NetworkBehaviour
{
    public float baseSpeed = 5f;
    public float speedMultiplier = 1.1f;
    private Rigidbody2D rb;

    public override void OnStartServer()
    {
        rb = GetComponent<Rigidbody2D>();
        Launch();
    }

    [Server]
    private void Launch()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(-0.5f, 0.5f);

        Vector2 direction = new Vector2(x, y).normalized;
        rb.linearVelocity = direction * baseSpeed;
    }

    [ServerCallback]

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PongMatchManager.Instance.RpcPlayRound("Bounce");

        if (collision.gameObject.CompareTag("Paddle"))
        {
            // 需求：根据击中球拍中心的位置调整角度
            float yOffset = transform.position.y - collision.transform.position.y;
            float paddleHeight = collision.collider.bounds.size.y;

            // 计算基于 -1 到 1 的偏移比例
            float yDirection = yOffset / paddleHeight;
            float xDirection = rb.linearVelocity.x > 0 ? -1 : 1; // 反弹

            Vector2 newDirection = new Vector2(xDirection, yDirection).normalized;

            float currentSpeed = rb.linearVelocity.magnitude * speedMultiplier;
            rb.linearVelocity = newDirection * currentSpeed;
        }
        else if (collision.gameObject.CompareTag("LeftWall"))
        {
            PongMatchManager.Instance.ScorePoint(2); // P2 得分
            NetworkServer.Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("RightWall"))
        {
            PongMatchManager.Instance.ScorePoint(1); // P1 得分
            NetworkServer.Destroy(gameObject);
        }
    }
}