using DebugUtils;
using FerryBoat;
using System;
using UnityEngine;


public class Block : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if ((targetLayer & (1 << collision.gameObject.layer)) != 0)
        {
            DevDebug.Log("Hit with the target ", DebugColor.Teal);

            Act.HitAction?.Invoke();
        }
    }
}
