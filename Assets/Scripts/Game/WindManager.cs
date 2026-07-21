using UnityEngine;

public class WindManager : MonoBehaviour
{
    [Header("Region")]
    [SerializeField] private Transform region;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [Range(0, 100f)]
    [SerializeField] private float attractionForce = 20f;

    [Range(0, 50f)]
    [SerializeField] private float maxSpeed = 15f;

    [Range(0, 10f)]
    [SerializeField] private float damping = 2f;       // Lower = more momentum

    [Range(0,10f)]
    [SerializeField] private float attractionRange = 10f;

    private Vector3 velocity;


    public bool isON = false;

    public void SetTarget(Transform mtarget) { target = mtarget; }

    public void SetDifficulty(DifficultyLevel level)
    {
        attractionForce = level switch
        {
            DifficultyLevel.easy => 8f,
            DifficultyLevel.medium => 15f,
            DifficultyLevel.hard => 25f,
           _ => 8f
        };
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.LogError("[WIND] target is null !!");
            return;
        }

        if (!isON) return;

        Vector3 direction =  region.position - target.position;
        float distance = direction.magnitude;

        attractionRange = attractionRange * 100f;

        if (distance > attractionRange)
        {
            Debug.LogError("[WIND] out of range");
            return;
        }

        // Simulate force
        Vector3 acceleration = direction.normalized * attractionForce;

        // Apply acceleration
        velocity += acceleration * Time.deltaTime;

        // Clamp max speed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Apply damping (friction)
        velocity *= Mathf.Exp(-damping * Time.deltaTime);

        Vector3 newPosition = target.position + velocity * Time.deltaTime;
        newPosition.y = target.position.y;

        target.position = newPosition;
    }
}
