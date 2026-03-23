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
        private Vector3 SourcePos;
        private Vector3 DestinationPos;
        private bool isMoving = false;
        public IEnumerator SetDestination(DockSet StartDock, DockSet EndDock)
        {
            agent = GetComponent<NavMeshAgent>();

            // Disable agent before moving, so it doesn't fight your position set
            agent.enabled = false;

            SourcePos = DockConfig.Instance.GetDock(StartDock).dockPosition;
            transform.position = SourcePos;

            // Re-enable after position is set — agent will warp to current position
            yield return new WaitForSeconds(1);

            DestinationPos = DockConfig.Instance.GetDock(EndDock).dockPosition;
            yield return MoveAgent();
        }
        private IEnumerator MoveAgent()
        {
            while (true)
            {
                agent.enabled = true;
                agent.SetDestination(DestinationPos);

                yield return new WaitUntil(() => !agent.pathPending &&
                                                 agent.remainingDistance <= agent.stoppingDistance);

                agent.enabled = false;
                SwapAndReturn();

                yield return new WaitForSeconds(1f); // pause at dock before returning
            }
        }
        void FixedUpdate()
        {
            if (!agent.enabled) return;
            Debug.DrawRay(transform.position, agent.velocity, Color.red);
        }
        private void SwapAndReturn()
        {
            // Swap source and destination
            (SourcePos, DestinationPos) = (DestinationPos, SourcePos);
        }
    }
}