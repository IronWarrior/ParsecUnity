using UnityEngine;

public class MovementDummy : MonoBehaviour
{
    public float speed = 5;

    private void Update()
    {
        Vector2 moveDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            moveDirection.y = 1;

        if (Input.GetKey(KeyCode.S))
            moveDirection.y = -1;

        if (Input.GetKey(KeyCode.A))
            moveDirection.x = -1;

        if (Input.GetKey(KeyCode.D))
            moveDirection.x = 1;

        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;
    }
}
