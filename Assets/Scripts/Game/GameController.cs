using FerryBoat.Actions;
using System;
using UnityEngine;

public class GameControlller : MonoBehaviour
{
    [SerializeField] GameObject playerPanel;

    private void OnEnable()
    {
        Act.EnableUser += EnableUserAction;
    }

    private void OnDisable()
    {
        Act.EnableUser -= EnableUserAction;
    }

    private void EnableUserAction(bool enable)
    {
        playerPanel.SetActive(enable);
    }
}
