using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed;

    private Vector3 rotation;

    private void Awake()
    {
        rotation = Random.insideUnitCircle * speed;
    }

    private void Update()
    {
        transform.rotation *= Quaternion.Euler(rotation * Time.deltaTime);
    }
}
