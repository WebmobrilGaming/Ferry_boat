using FerryBoat.Actions;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    [Header("Endline")]
    [SerializeField] private GameObject Boat;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Boat)
        {
            Debug.LogWarning("EndPoint Reached - Game Over");
            Act.EndPointReached?.Invoke();
        }
    }

}
