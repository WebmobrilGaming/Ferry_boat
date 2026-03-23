using System.Collections;
using Ferry.Config;
using UnityEngine;
namespace Ferry.Ship
{
    public class ShipMotor : MonoBehaviour
    {
        public float speed = 10f;
        public float turnSpeed = 50f;
        public DockSet StartDock;
        public DockSet EndDock;
        private IShipController controller;

        void Awake()
        {
            controller = GetComponent<IShipController>();
            StartCoroutine(controller.SetDestination(StartDock,EndDock));
        }
    }
    public interface IShipController
    {
        IEnumerator SetDestination(DockSet start,DockSet target);
    }
}