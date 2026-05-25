using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplayTest : MonoBehaviour
{
    [SerializeField] private GameObject[] cams;
    [SerializeField] private GameObject[] gameplayCam;
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
       cams[1].gameObject.SetActive(false);
       cams[2].gameObject.SetActive(false);
        gameplayCam[0].SetActive(true);
        gameplayCam[1].SetActive(true);
    }

    public void EnableCams()
    {
        gameplayCam[0].SetActive(false);
        gameplayCam[1].SetActive(false);
        for (int i = 1; i < cams.Length; i++)
        {
            cams[i].SetActive(true);
            
        }
    }
}
