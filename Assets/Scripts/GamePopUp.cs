using System;
using TMPro;
using UnityEngine;

public class GamePopUp : MonoBehaviour
{
    static GamePopUp instance;

    public static GamePopUp Instance {  get { return instance; } }

    [SerializeField] GameObject panelFinal;
    [SerializeField] TMP_Text mFinalText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
  
    public void FinalPopUp(string message)
    {
        panelFinal.SetActive(true);
        mFinalText.text = message;
    }
}
