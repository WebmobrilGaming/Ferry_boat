using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class TowedBarge : MonoBehaviour
{
    [Header("Boat")]
    public Transform boatTransform;
    public Transform joinPoint;
    public float ropeLength = 5f;

    public enum BoatAxis { Forward, Back, Right, Left }

    [Header("Stern Axis")]
    public BoatAxis sternAxis = BoatAxis.Back;

    [Header("Position Offset")]
    [Tooltip("Fine-tune barge start position in boat's LOCAL space.\n" +
             "X = left/right  Y = up/down  Z = forward/back\n" +
             "Adjust X until barge is perfectly centered behind stern.")]
    public Vector3 attachOffset = Vector3.zero;

    [Header("Rope Feel")]
    public float pullStrength = 35f;
    public float pullDamping = 3f;
    public float pathDelay = 0.15f;

    [Header("Rotation")]
    public float yawSpeed = 3f;
    public float maxRoll = 6f;

    [Header("Drag")]
    public float linearDrag = 1.5f;

    private struct PathPoint { public Vector3 pos; public float time; }

    private List<PathPoint> _path = new List<PathPoint>();
    private Rigidbody _rb;
    private float _prevYaw;
    private float _yawRate;
    private Quaternion _smoothBoatRot;
    private float _bargeY;

    private Vector3 SternDir => sternAxis switch
    {
        BoatAxis.Forward => boatTransform.forward,
        BoatAxis.Back => -boatTransform.forward,
        BoatAxis.Right => boatTransform.right,
        BoatAxis.Left => -boatTransform.right,
        _ => -boatTransform.forward
    };

    // Stern world position including the local offset
    private Vector3 SternWorldPos
    {
        get
        {
            Vector3 base_pos = boatTransform.position + SternDir * ropeLength;
            // Apply offset in boat's local space so it rotates with the boat
            base_pos += boatTransform.TransformDirection(attachOffset);
            return base_pos;
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.linearDamping = linearDrag;
        _rb.angularDamping = 0f;
        _rb.useGravity = false;

        _rb.constraints = RigidbodyConstraints.FreezePositionY
                        | RigidbodyConstraints.FreezeRotationX
                        | RigidbodyConstraints.FreezeRotationY
                        | RigidbodyConstraints.FreezeRotationZ;

        _smoothBoatRot = boatTransform.rotation;
        _prevYaw = boatTransform.eulerAngles.y;
        _bargeY = transform.position.y;

        Vector3 startPos = SternWorldPos;
        startPos.y = _bargeY;

        for (int i = 0; i < 60; i++)
        {
            _path.Add(new PathPoint
            {
                pos = startPos,
                time = Time.time - pathDelay + i * (pathDelay / 60f)
            });
        }

        joinPoint.position = startPos;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector3 sternPos = SternWorldPos;
        sternPos.y = _bargeY;

        _path.Add(new PathPoint { pos = sternPos, time = Time.time });

        float keepFrom = Time.time - pathDelay - 0.5f;
        while (_path.Count > 2 && _path[0].time < keepFrom)
            _path.RemoveAt(0);

        _smoothBoatRot = Quaternion.Slerp(
            _smoothBoatRot, boatTransform.rotation, dt * 5f);

        float currentYaw = boatTransform.eulerAngles.y;
        _yawRate = Mathf.DeltaAngle(_prevYaw, currentYaw) / dt;
        _yawRate = Mathf.Clamp(_yawRate, -90f, 90f);
        _prevYaw = currentYaw;
    }

    private void FixedUpdate()
    {
        if (boatTransform == null) return;

        Vector3 anchor = SamplePath(Time.time - pathDelay);
        Vector3 toAnchor = anchor - joinPoint.position;
        float dist = toAnchor.magnitude;

        if (dist > 0.05f)
        {
            Vector3 dir = toAnchor.normalized;
            Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            float pull = Mathf.Clamp(dist * pullStrength, 0f, 40f);
            Vector3 damping = Vector3.Project(flatVel, dir) * pullDamping;
            _rb.AddForce(dir * pull - damping, ForceMode.Acceleration);
        }
    }

    private void LateUpdate()
    {
        if (boatTransform == null) return;

        float dt = Time.deltaTime;
        Vector3 toBoat = boatTransform.position - transform.position;
        toBoat.y = 0f;

        if (toBoat.magnitude > 0.05f)
        {
            Quaternion targetYaw = Quaternion.LookRotation(toBoat.normalized);
            float roll = Mathf.Clamp(-_yawRate * 0.08f, -maxRoll, maxRoll);
            Quaternion finalRot = targetYaw * Quaternion.Euler(0f, 0f, roll);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, finalRot, dt * yawSpeed);
        }
    }

    private Vector3 SamplePath(float targetTime)
    {
        if (_path.Count == 0)
        {
            Vector3 f = SternWorldPos;
            f.y = _bargeY;
            return f;
        }

        if (targetTime <= _path[0].time) return _path[0].pos;
        if (targetTime >= _path[_path.Count - 1].time) return _path[_path.Count - 1].pos;

        for (int i = 1; i < _path.Count; i++)
        {
            if (_path[i].time >= targetTime)
            {
                float span = _path[i].time - _path[i - 1].time;
                float t = span > 0f ? (targetTime - _path[i - 1].time) / span : 0f;
                return Vector3.Lerp(_path[i - 1].pos, _path[i].pos, t);
            }
        }

        return _path[_path.Count - 1].pos;
    }

    private void OnDrawGizmosSelected()
    {
        if (boatTransform == null) return;

        Vector3 stern = SternWorldPos;
        stern.y = Application.isPlaying ? _bargeY : transform.position.y;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(stern, 0.3f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, stern);

        // Show stern direction
        Gizmos.color = Color.red;
        Gizmos.DrawRay(boatTransform.position, SternDir * 4f);

        if (Application.isPlaying && _path.Count > 1)
        {
            Gizmos.color = Color.cyan;
            for (int i = 1; i < _path.Count; i++)
                Gizmos.DrawLine(_path[i - 1].pos, _path[i].pos);
        }
    }
}