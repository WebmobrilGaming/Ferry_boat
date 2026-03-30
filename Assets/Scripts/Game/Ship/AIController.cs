using System.Collections;
using DG.Tweening;
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
        private Velocity currentVelocityLevel;

        private Vector3 sourcePos;
        private Vector3 destinationPos;

        [SerializeField] private float navMeshSampleDistance = 10f;

        private bool isDecelerating = false;

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
        public void SetPath(DockSet start, DockSet end)
        {
            this.StartDock = start;
            this.EndDock = end;
            shipConfig = ShipConfigController.Instance.GetShipConfig(shipType);
            currentVelocityLevel = shipConfig.velocitiesLevels[(int)difficultyLevel];

            agent.speed = currentVelocityLevel.shipSpeed;
            agent.angularSpeed = currentVelocityLevel.angularSpeed;
            agent.acceleration = currentVelocityLevel.acceleration;
            agent.autoBraking = true;

            // stoppingDistance drives how early the agent starts braking —
            // reusing deceleration value: higher decel = starts braking earlier
            agent.stoppingDistance = Mathf.Clamp(currentVelocityLevel.deceleration, 1f, 10f);

            sourcePos = DockConfig.Instance.GetDock(StartDock).dockPosition;
            destinationPos = DockConfig.Instance.GetDock(EndDock).dockPosition;
        }
        private void StartShip()
        {
            StartCoroutine(RunShuttle());
        }

        private IEnumerator RunShuttle()
        {
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

                // restore full speed at the start of each leg
                agent.speed = currentVelocityLevel.shipSpeed;
                isDecelerating = false;

                agent.SetDestination(targetPoint);

                yield return new WaitUntil(() =>
                    !agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance);

                agent.ResetPath();

                (sourcePos, destinationPos) = (destinationPos, sourcePos);

                yield return new WaitForSeconds(1f);
            }
        }

        private void FixedUpdate()
        {
            if (agent == null || !agent.isOnNavMesh) return;

            Debug.DrawRay(transform.position, agent.velocity, Color.red);

            if (currentVelocityLevel == null) return;

            // once within braking range, manually decelerate using config value
            bool nearDestination = !agent.pathPending &&
                                   agent.remainingDistance <= agent.stoppingDistance + 3f;

            if (nearDestination)
            {
                isDecelerating = true;
            }

            if (isDecelerating)
            {
                agent.speed = Mathf.MoveTowards(
                    agent.speed,
                    0f,
                    currentVelocityLevel.deceleration * Time.fixedDeltaTime
                );
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
    }
}