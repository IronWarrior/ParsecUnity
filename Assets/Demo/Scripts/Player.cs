using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed = 4;

    [SerializeField]
    float acceleration = 60;

    [SerializeField]
    float turnSpeed = 180;

    private PlayerInput playerInput;

    private Vector3 velocity;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 worldMoveInput = Vector3.ClampMagnitude(new Vector3(moveInput.x, 0, moveInput.y), 1);

        velocity = Vector3.MoveTowards(velocity, worldMoveInput * speed, acceleration * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;

        if (moveInput != Vector2.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(velocity), turnSpeed * Time.deltaTime);
    }

    public void SetColor(Color color)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = color;
        }
    }
}
