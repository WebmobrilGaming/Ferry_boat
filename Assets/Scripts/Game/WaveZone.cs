using Ferry.Motion;
using UnityEngine;

public class WaveZone : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    private Collider dockCollider;
     private void Awake()
    {
        dockCollider = GetComponent<Collider>();

    }
    void OnTriggerEnter(Collider other)
    {
        if (!IsTargetLayer(other)) return;

        var waveObj = other.GetComponentInParent<WaveMotion>();
        if (waveObj == null) return;

        Debug.Log("Level changed");
        waveObj.SetTargetLevel();
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsTargetLayer(other)) return;

        var waveObj = other.GetComponentInParent<WaveMotion>();
        if (waveObj == null) return;

        Debug.Log("Level reset");
        waveObj.ResetLevel();
    }
    private bool IsTargetLayer(Collider other)
    {
        return (targetLayer & (1 << other.gameObject.layer)) != 0;
    }
}
