using System;
using Unity.Cinemachine;
using UnityEngine;



[RequireComponent(typeof(CinemachineClearShot))]
public class Cam : MonoBehaviour
{
    CinemachineClearShot shot;

    [SerializeField] CamType camType;
    public CamType Type => camType;

    private void Awake()
    {
        shot = GetComponent<CinemachineClearShot>();
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
