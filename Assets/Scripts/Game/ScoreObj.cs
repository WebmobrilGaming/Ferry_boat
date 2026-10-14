using DebugUtils;
using FerryBoat.Actions;
using FerryBoat.Store;
using UnityEngine;

public class ScoreObj : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] BoxCollider boxCollider;

    private void OnEnable()
    {
        boxCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTargetLayer(other))
        {
            return;
        }

        DevDebug.Log("Scored 50 !!!", DebugColor.Green);

        Score_System.Instance.Set(50, Data.time);

        Act.ShowWarn?.Invoke("You have crossed the barrier! Congratulations!");

        boxCollider.enabled = false;
    }

    private bool IsTargetLayer(Collider other)
    {
        return (targetLayer & (1 << other.gameObject.layer)) != 0;
    }
}
