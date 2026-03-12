using UnityEditor;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoatController : MonoBehaviour,IHelem,IGear
{
    [SerializeField] HelmController helmController;
    [SerializeField] Gear mGear;

    [Space]
    [SerializeField] Button mGearBtn;

    [Header("Settings:")]
    [Range(0,1f)]
    [SerializeField] float rotationMultiplier = 0.3f;

    [Range(0, 10f)]
    [SerializeField] float mSpeed = 5.0f;


    [SerializeField] bool isControl;

    Quaternion startRotation;

    private void OnEnable()
    {
        helmController.callback = this;
        mGear.callback = this;

        mGearBtn.onClick.AddListener(GearAction);

        startRotation = transform.localRotation;
    }

    void GearAction()
    {
        mGearBtn.interactable = false;
        isControl = false;

        mGear.Change(!mGear.Stat);
    }

    public void GearChange()
    {
        isControl = true;
        mGearBtn.interactable = true;
    }

    private void Update()
    {
        if (Keyboard.current.leftArrowKey.isPressed)
            helmController.Direct(HelmDirection.left);

        if (Keyboard.current.rightArrowKey.isPressed)
            helmController.Direct(HelmDirection.right);

        if (Keyboard.current.leftArrowKey.wasReleasedThisFrame || Keyboard.current.rightArrowKey.wasReleasedThisFrame)
        {
          //  Debug.Log("Released helm !!!");
            helmController.StopRotation();
        }

        if(mGear.Stat)
            transform.position -= transform.forward * mSpeed * Time.deltaTime;
    }

    private float currentRotation = 0f;
    private float lastHelmZ = 0f;
    private bool isFirstUpdate = true;

    public void Rotate(float zValue)
    {
        if (isFirstUpdate)
        {
            lastHelmZ = zValue;
            isFirstUpdate = false;
            return;
        }

        float delta = Mathf.DeltaAngle(lastHelmZ, zValue);
        lastHelmZ = zValue;

        currentRotation += delta * rotationMultiplier;

        transform.localRotation = startRotation * Quaternion.Euler(0, currentRotation, 0);

        Debug.Log($"Delta: {delta} | Accumulated: {currentRotation}");
    }
}
