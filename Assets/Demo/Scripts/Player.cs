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

    [SerializeField]
    AudioClip fireAudioClip;

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

        GetComponent<AudioSource>().PlayOneShot(fireAudioClip);
    }

    private void Update()
    {
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector2 lookDirection = playerInput.actions["LookDirection"].ReadValue<Vector2>();
        Vector2 lookTarget = playerInput.actions["LookTarget"].ReadValue<Vector2>();

        Vector3 worldMoveInput = Vector3.ClampMagnitude(new Vector3(moveInput.x, 0, moveInput.y), 1);
        Vector3 worldLookDirection = new Vector3(lookDirection.x, 0, lookDirection.y);

        velocity = Vector3.MoveTowards(velocity, worldMoveInput * speed, acceleration * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;

        if (moveInput != Vector2.zero)
            chassis.rotation = Quaternion.RotateTowards(chassis.rotation, Quaternion.LookRotation(worldMoveInput), turnSpeed * Time.deltaTime);

        if (lookDirection != Vector2.zero)
            turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.LookRotation(worldLookDirection), turnSpeed * Time.deltaTime);

        if (lookTarget != Vector2.zero)
        {
            Ray ray = Camera.main.ScreenPointToRay(lookTarget);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);
                Vector3 to = point - turret.position;
                to.y = 0;

                turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.LookRotation(to), turnSpeed * Time.deltaTime);
            }
        }
    }

    public void SetColor(Color color)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = color;
        }
    }
}
