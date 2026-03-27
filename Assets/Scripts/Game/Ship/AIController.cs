using System.Collections;
using Ferry.Config;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;

namespace Ferry.Ship
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AIController : MonoBehaviour
    {
        public DockSet StartDock;
        public DockSet EndDock;
        public ShipType shipType;
        public DifficultyLevel difficultyLevel;

        private NavMeshAgent agent;
        private ShipConfig shipConfig;

        private Vector3 sourcePos;
        private Vector3 destinationPos;

        [SerializeField] private float navMeshSampleDistance = 10f;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            difficultyLevel = GetDifficultyLevel();
        }

        private void OnEnable()
        {
            BoatController.OnBoatStartEvent += StartShip;
        }

        private void OnDisable()
        {
            BoatController.OnBoatStartEvent -= StartShip;
        }

        private void StartShip()
        {
            if (difficultyLevel == DifficultyLevel.hard && shipType == ShipType.TankerShip)
                return;

            StartCoroutine(RunShuttle());
        }

        private IEnumerator RunShuttle()
        {
            // Load config
            shipConfig = ShipConfigController.Instance.GetShipConfig(shipType);

            agent.speed = shipConfig.velocitiesLevels[(int)difficultyLevel].shipSpeed;
            agent.angularSpeed = shipConfig.velocitiesLevels[(int)difficultyLevel].angularSpeed;
            agent.acceleration = shipConfig.velocitiesLevels[(int)difficultyLevel].acceleration;

            // Get dock positions
            sourcePos = DockConfig.Instance.GetDock(StartDock).dockPosition;
            destinationPos = DockConfig.Instance.GetDock(EndDock).dockPosition;

            // Snap to valid NavMesh position
            Vector3 startPoint = GetNearestNavMeshPoint(sourcePos);
            if (startPoint == Vector3.zero)
            {
                Debug.LogError($"[{shipType}] Invalid start position on NavMesh");
                yield break;
            }

            agent.Warp(startPoint);

            yield return new WaitForSeconds(0.5f);

            while (true)
            {
                Vector3 targetPoint = GetNearestNavMeshPoint(destinationPos);

                if (targetPoint == Vector3.zero)
                {
                    Debug.LogError($"[{shipType}] Invalid destination on NavMesh");
                    yield break;
                }

                if (!agent.isOnNavMesh)
                {
                    Debug.LogError($"[{shipType}] Agent is not on NavMesh");
                    yield break;
                }

                agent.SetDestination(targetPoint);

                // Wait until reached
                yield return new WaitUntil(() =>
                    !agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance);

                agent.ResetPath();

                // Swap docks
                (sourcePos, destinationPos) = (destinationPos, sourcePos);

                yield return new WaitForSeconds(1f);
            }
        }

        private Vector3 GetNearestNavMeshPoint(Vector3 target)
        {
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
                return hit.position;

            return Vector3.zero;
        }

        private DifficultyLevel GetDifficultyLevel()
        {
            if (PlayerPrefs.HasKey("Settings"))
            {
                string json = PlayerPrefs.GetString("Settings");
                var loaded = JsonConvert.DeserializeObject<SettingsData>(json);
                return loaded.level;
            }
            return DifficultyLevel.easy;
        }

        private void FixedUpdate()
        {
            if (agent != null && agent.isOnNavMesh)
            {
                Debug.DrawRay(transform.position, agent.velocity, Color.red);
            }
        }
    }
}