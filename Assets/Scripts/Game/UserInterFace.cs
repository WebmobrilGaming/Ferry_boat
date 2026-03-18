using DebugUtils;
using FerryBoat;
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
        scrollAction.performed += OnScroll;
    }

    private void OnEnable()
    {
        scrollAction.Enable();
    }

    private void OnDisable()
    {
        scrollAction.Disable();
    }

    private void OnDestroy()
    {
        scrollAction.performed -= OnScroll;
    }

    public void SetRange(float range)
    {
        rangeMin = 0;
        rangeMax = range;
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        Vector2 scroll = ctx.ReadValue<Vector2>();

       float mScrollVal = scroll.y;

        mScrollRange = Mathf.Clamp(mScrollRange + mScrollVal * sensitivity, rangeMin, rangeMax);

        Act.SpeedChange?.Invoke(mScrollRange);
    }
}
