using System.Collections;
using FerryBoat.Actions;
using UnityEngine;

public class EndPoint : MonoBehaviour
{
    [Header("Endline")]
    [SerializeField] private GameObject Boat;
    [SerializeField] BoatController boatController;

    void Awake()
    {
        boatController = FindAnyObjectByType<BoatController>().GetComponent<BoatController>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Boat)
        {
            Debug.LogWarning("EndPoint Reached");
            boatController.PerformUTurn();
            StartCoroutine(EndPointPopUp());
        }
    }

    IEnumerator EndPointPopUp()
    {
        GamePopUp.Instance.FinalPopUp("EndPoint Reached");
        yield return new WaitForSecondsRealtime(2f);
        GamePopUp.Instance.ClosePanel();
    }

}
