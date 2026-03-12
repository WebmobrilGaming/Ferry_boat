using UnityEngine;

using UnityEngine.InputSystem;

public class BoatController : MonoBehaviour
{
    [SerializeField] HelmController helmController;

    private void Update()
    {
        if (Keyboard.current.leftArrowKey.isPressed)
            helmController.Direct(HelmDirection.left);

        if (Keyboard.current.rightArrowKey.isPressed)
            helmController.Direct(HelmDirection.right);

        if (Keyboard.current.leftArrowKey.wasReleasedThisFrame || Keyboard.current.rightArrowKey.wasReleasedThisFrame)
        {
            Debug.Log("Released helm !!!");
            helmController.StopRotation();
        }
    }
}
