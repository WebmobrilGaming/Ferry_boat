using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform,false);
    }
#endif
    public float minimum;
    public float maximum;
    public float current;
    public Image mask;

    private void Update()
    {
        GetCurrentFill();
    }
    private void GetCurrentFill()
    {
        current=Math.Clamp(current,minimum,maximum);
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset/ maximumOffset;
        mask.fillAmount = fillAmount;
    }
}
