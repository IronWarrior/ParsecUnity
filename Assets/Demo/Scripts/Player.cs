using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]
    Transform turret;

    [SerializeField]
    Transform chassis;

    [SerializeField]
    float speed = 4;

    [SerializeField]
    float acceleration = 60;

    [SerializeField]
    float turnSpeed = 180;

    [SerializeField]
    Rigidbody projectilePrefab;

    [SerializeField]
    float fireSpeed = 10;

    private PlayerInput playerInput;

    private Vector3 velocity;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Fire"].performed += Fire;
    }

    private void Fire(InputAction.CallbackContext obj)
    {
        Rigidbody projectile = Instantiate(projectilePrefab, turret.transform.position + turret.transform.forward * 0.5f, Quaternion.identity);
        projectile.velocity = fireSpeed * turret.transform.forward;

        Destroy(projectile.gameObject, 2f);
    }

    private void Update()
    {
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector2 lookDirection = playerInput.actions["LookDirection"].ReadValue<Vector2>();

        Vector3 worldMoveInput = Vector3.ClampMagnitude(new Vector3(moveInput.x, 0, moveInput.y), 1);
        Vector3 worldLookDirection = new Vector3(lookDirection.x, 0, lookDirection.y);

        velocity = Vector3.MoveTowards(velocity, worldMoveInput * speed, acceleration * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;

        if (moveInput != Vector2.zero)
            chassis.rotation = Quaternion.RotateTowards(chassis.rotation, Quaternion.LookRotation(worldMoveInput), turnSpeed * Time.deltaTime);

        if (lookDirection != Vector2.zero)
            turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.LookRotation(worldLookDirection), turnSpeed * Time.deltaTime);
    }

    public void SetColor(Color color)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = color;
        }
    }
}
