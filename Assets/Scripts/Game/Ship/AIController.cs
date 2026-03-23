using System.Collections;
using Ferry.Config;
using UnityEngine;
using UnityEngine.AI;
namespace Ferry.Ship
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIController : MonoBehaviour, IShipController
    {
        private NavMeshAgent agent;
        public IEnumerator SetDestination(DockSet StartDock, DockSet EndDock)
        {
            agent = GetComponent<NavMeshAgent>();

            // Disable agent before moving, so it doesn't fight your position set
            agent.enabled = false;

            Vector3 startPos = DockConfig.Instance.GetDock(StartDock).dockPosition;
            transform.position = startPos;

            // Re-enable after position is set — agent will warp to current position
            agent.enabled = true;

            yield return new WaitForSeconds(1);

            Vector3 endPos = DockConfig.Instance.GetDock(EndDock).dockPosition;
            agent.SetDestination(endPos);
        }
        void FixedUpdate()
        {
            Debug.DrawRay(transform.position, agent.velocity, Color.red);
        }
    }
}