using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private void HandleMovement()
    {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveHorizontal * 0.1f, moveVertical * 0.1f, 0f);

            transform.position += movement;
        }
    }

    private void Update()
    {
        HandleMovement();
    }
}
