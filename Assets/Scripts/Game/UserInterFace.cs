using DebugUtils;
using FerryBoat;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInterFace : MonoBehaviour
{
    [SerializeField] InputAction scrollAction;

    [Space]
    [SerializeField] float mScrollRange;

    float rangeMin = 0f;
    float rangeMax = 10f;
    [SerializeField] float sensitivity = 1f;

    private void Awake()
    {
        rangeMax = -1;
        scrollAction.performed += OnScroll;
    }

    private void OnEnable()
    {
        scrollAction.Enable();

        Act.SpeedInit += SetRange;
        Act.ReachedDestination += LevelFinish;
        Act.BoatDestroyedAction += BoatDestroyed;
    }

    private void OnDisable()
    {
        scrollAction.Disable();

        Act.SpeedInit -= SetRange;
        Act.ReachedDestination -= LevelFinish;
    }

    private void BoatDestroyed()
    {
        Time.timeScale = 0;
        GamePopUp.Instance.FinalPopUp("Boat destroyed !!");
    }


    private void OnDestroy()
    {
        scrollAction.performed -= OnScroll;
    }

    private void LevelFinish()
    {
        Time.timeScale = 0;
        GamePopUp.Instance.FinalPopUp("Destination Reached");
    }

    public void SetRange(float range)
    {
        rangeMin = 0;
        rangeMax = range;
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        if (rangeMax <= 0)
            return;

        Vector2 scroll = ctx.ReadValue<Vector2>();

        float mScrollVal = -1 * scroll.y;

        mScrollRange = Mathf.Clamp(mScrollRange + mScrollVal * sensitivity, rangeMin, rangeMax);

        Act.SpeedChange?.Invoke(mScrollRange);
    }
}
