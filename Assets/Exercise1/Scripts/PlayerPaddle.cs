using Mirror;
using UnityEngine;

public class PlayerPaddle : NetworkBehaviour
{
    public float speed = 10f;
    public float yBoundary = 4f;

    void Update()
    {
        // 只有本地玩家才能控制这个球拍
        if (!isLocalPlayer) return;

        float move = Input.GetAxisRaw("Vertical");
        Vector3 newPos = transform.position + new Vector3(0, move * speed * Time.deltaTime, 0);

        // 限制在屏幕内
        newPos.y = Mathf.Clamp(newPos.y, -yBoundary, yBoundary);
        transform.position = newPos;

        // 注意：因为在 NetworkTransform 中勾选了 Client Authority，
        // 这里的 Transform 修改会自动通过底层 Command 同步给服务器和其他客户端。
    }
}