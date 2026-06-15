using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public interface IHelem
{
    public void Rotate(float zValue);
}

public class HelmController : MonoBehaviour
{
    Tween rotateTween;

    [SerializeField] Quaternion currentRotation;

    [Space]
    [SerializeField] bool isRotate = false;

    public IHelem callback;

    public void Direct (HelmDirection direction)
    {
        if (isRotate)
            return;

        isRotate = true;

        switch (direction)
        {    
            case HelmDirection.right:
                rotateTween = transform
                               .DOLocalRotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
                               .SetRelative()
                               .OnUpdate(() =>
                               {
                                   if (callback == null) return;


                                   float helmZ = transform.localEulerAngles.z;

                                   if (helmZ > 180)
                                       helmZ -= 360;

                                   callback.Rotate(helmZ);
                               })
                               .SetEase(Ease.Linear)
                               .SetLoops(-1);
                break;

            case HelmDirection.left:
                rotateTween = transform
                             .DOLocalRotate(new Vector3(0, 0, -360), 2f, RotateMode.FastBeyond360)
                             .SetRelative()
                             .OnUpdate(() =>
                              {
                                  if (callback == null) return;

                                  float helmZ = transform.localEulerAngles.z;

                                  if (helmZ > 180)
                                      helmZ -= 360;

                                  callback.Rotate(helmZ);
                              })
                             .SetEase(Ease.Linear)
                             .SetLoops(-1);
                break;
        }
    }
    public void SetWheelAngle(float wheelInput)
    {
        

        float maxHelmAngle = 360f;

        float targetAngle = wheelInput * maxHelmAngle;

        transform.localRotation =
            Quaternion.Euler(0, 0, targetAngle);
    }

    public void StopRotation()
    {
        if (rotateTween == null) return;

        DOTween.To(
            () => rotateTween.timeScale,
            x => rotateTween.timeScale = x,
            0f,
            0.6f   
        )
        .OnComplete(() =>
        {
            rotateTween.Kill();
            rotateTween = null;

            isRotate = false;
        });
    }
}

public enum HelmDirection { left, right }