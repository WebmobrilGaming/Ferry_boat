using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(Animator))]
[RequireComponent(typeof(PathFollower))]
public class NPC : MonoBehaviour
{
    Animator animator;

    [SerializeField] PathFollower pathFollower;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        pathFollower.OnPathComplete += PathCompleteAction;
    }

    private void OnDisable()
    {
        pathFollower.OnPathComplete -= PathCompleteAction;
    }

    private void PathCompleteAction()
    {
        animator.Play("Idle");
    }
}
