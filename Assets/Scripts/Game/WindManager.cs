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

    [Range(0,100f)]
    [SerializeField] private float attractionRange = 10f;

    private Vector3 velocity;


    public bool isON = false;

    public void SetTarget(Transform mtarget) { target = mtarget; }

    public void SetDifficulty(DifficultyLevel level)
    {
        attractionForce = level switch
        {
            DifficultyLevel.easy => 6f,
            DifficultyLevel.medium => 12f,
            DifficultyLevel.hard => 20f,
           _ => 6f
        };
    }

    private void Update()
    {
        if (target == null)
        {
            Debug.LogError("Target is NULL");
            return;
        }

        Debug.Log("Target OK");

        if (!isON)
        {
            Debug.Log("Wind OFF");
            return;
        }

        Debug.Log("Wind ON");

        Vector3 direction = region.position - target.position;
        float distance = direction.magnitude;

        Debug.Log($"Distance: {distance}");

        attractionRange = attractionRange * 1000f;

        if (distance > attractionRange)
        {
            Debug.Log("Out of Range");
            return;
        }

        Debug.Log($"Inside Range >>> FORCE: {attractionForce}");

        target.position = Vector3.MoveTowards(
                                    target.position,
                                    region.position,
                                    attractionForce * Time.deltaTime
                                             );
    }
}
