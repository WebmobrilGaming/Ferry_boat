using FerryBoat.Actions;
using GF;
using UnityEngine;
using UnityEngine.Video;

public class DockPassCheker : MonoBehaviour
{
    [SerializeField] private GameObject ferryBoat;
    public static bool passedStartingDock;

    void OnEnable()
    {
        passedStartingDock = false;
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == ferryBoat)
        {
            passedStartingDock = true;
            PopUp.Show("You Are Leaving the Starting Dock, Safe Journey!!");
            Debug.LogWarning("starting dock Passed");
        }
    }
}
