using System.Collections;
using DG.Tweening;
using Ferry.Config;
using Ferry.Motion;
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

            // IMPORTANT: Disable auto rotation
            agent.updateRotation = false;
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
            var waveMotion=GetComponent<WaveMotion>();
            if (waveMotion != null)
            {
                waveMotion.SetLevel(difficultyLevel);
            }
            shipConfig = ShipConfigController.Instance.GetShipConfig(shipType);
            currentVelocityLevel = shipConfig.velocitiesLevels[(int)difficultyLevel];

            agent.speed = currentVelocityLevel.shipSpeed;
            agent.angularSpeed = currentVelocityLevel.angularSpeed;
            agent.acceleration = currentVelocityLevel.acceleration;
            agent.autoBraking = true;

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

            // FACE correct direction immediately at spawn
            Vector3 initialDir = (destinationPos - startPoint).normalized;
            if (initialDir != Vector3.zero)
                transform.forward = initialDir;

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

                // restore speed
                agent.speed = currentVelocityLevel.shipSpeed;
                isDecelerating = false;

                // FORCE forward direction BEFORE movement (NO ROTATE-FIRST)
                Vector3 dir = (targetPoint - transform.position).normalized;
                if (dir != Vector3.zero)
                    transform.forward = dir;

                agent.SetDestination(targetPoint);

                yield return new WaitUntil(() =>
                    !agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance);

                agent.ResetPath();

                // swap source/destination
                (sourcePos, destinationPos) = (destinationPos, sourcePos);

                yield return new WaitForSeconds(1f);
            }
        }

        private void Update()
        {
            if (agent == null || !agent.isOnNavMesh) return;

            // Smooth rotation WHILE moving (optional but recommended)
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(agent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    Time.deltaTime * 2f
                );
            }
        }

        private void FixedUpdate()
        {
            if (agent == null || !agent.isOnNavMesh) return;

            Debug.DrawRay(transform.position, agent.velocity, Color.red);

            if (currentVelocityLevel == null) return;

            bool nearDestination = !agent.pathPending &&
                                   agent.remainingDistance <= agent.stoppingDistance + 3f;

            if (nearDestination)
                isDecelerating = true;

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