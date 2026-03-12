using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HelmController : MonoBehaviour
{

    Tween rotateTween;

    [SerializeField] Quaternion currentRotation;

    [Space]
    [SerializeField] bool isRotate = false;

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
                               .OnStart(() => { Debug.Log("Start the rotation !!!"); })
                               .SetRelative()
                               .SetEase(Ease.Linear)
                               .OnUpdate(() => 
                               {
                                   Debug.Log("Roatation is enabled !!!");

                                   //if (!isRotate)
                                   //{
                                   //    currentRotation = transform.rotation;

                                   //    rotateTween.Kill();
                                   //    rotateTween = null;

                                   //    transform.DORotateQuaternion(currentRotation, 0.3f)
                                   //             .SetEase(Ease.OutQuad);

                                   //    return;
                                   //}

                                   
                               })
                               .SetLoops(-1);
                break;

           case HelmDirection.left:
                rotateTween = transform
                             .DOLocalRotate(new Vector3(0, 0, -360), 2f, RotateMode.FastBeyond360)
                             .SetRelative()
                             .SetEase(Ease.Linear)
                             .OnUpdate(() =>
                                {
                                    Debug.Log("Roatation is enabled !!!");

                                    //if (!isRotate)
                                    //{
                                    //    currentRotation = transform.rotation;

                                    //    rotateTween.Kill();
                                    //    rotateTween = null;

                                    //    transform.DORotateQuaternion(currentRotation, 0.3f)
                                    //            .SetEase(Ease.OutQuad);
                                    //    return;
                                    //}

                                   
                                })
                             .SetLoops(-1);
                break;
        }
    }

    public void StopRotation()
    {
        if (rotateTween == null) return;

        // Gradually slow down the rotation
        DOTween.To(
            () => rotateTween.timeScale,
            x => rotateTween.timeScale = x,
            0f,
            0.6f   // slowdown duration
        )
        .OnComplete(() =>
        {
            rotateTween.Kill();
            rotateTween = null;

            isRotate = false;

            //transform.DORotateQuaternion(currentRotation, 0.3f)
            //                                   .SetEase(Ease.OutQuad);

            //currentRotation = transform.rotation;
        });
    }
}

public enum HelmDirection { left, right }