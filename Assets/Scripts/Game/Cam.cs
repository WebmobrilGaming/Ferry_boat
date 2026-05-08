using System;
using Unity.Cinemachine;
using UnityEngine;



[RequireComponent(typeof(CinemachineCamera))]
public class Cam : MonoBehaviour
{
    [SerializeField] CinemachineCamera shot;

    [SerializeField] CamType camType;
    public CamType Type => camType;

    private void Awake()
    {
        shot = GetComponent<CinemachineCamera>();
    }

    public void SetPriority(bool enable)
    {
        if (shot == null)
        {
            Debug.LogError($"[Cam] shot is null on {gameObject.name}", this);
            return;
        }

        shot.Priority = new PrioritySettings
        {
            Enabled = true,
            Value = enable ? 20 : 10
        };
    }
}
