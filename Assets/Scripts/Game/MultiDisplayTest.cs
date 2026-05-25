using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplayTest : MonoBehaviour
{
    [SerializeField] private GameObject[] cams;
    // Start is called before the first frame update
    void OnEnable()
    {
        EnableCams();
    }
    void Start()
    {
        for(int i =0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        Debug.LogWarning("Detected display : "+ Display.displays.Length);
    }
    void OnDisable()
    {   
        // Debug.LogWarning("disabling left and right camera");
        // for(int i = 1; i < Display.displays.Length; i++)
        // {
        //      cams[i].SetActive(false);
        //      Debug.LogWarning($"cam {i} disabled");
        // }
    }

    public void EnableCams()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            cams[i].SetActive(true);
        }
    }
}
