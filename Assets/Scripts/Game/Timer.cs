using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float duration = 60f;

    [SerializeField] private float _remaining;
    [SerializeField] private bool _running;

    public bool IsRunning => _running;
    public float Remaining => _remaining;

    void Start() => Reset();

    public  Action OnCompleteAct;
    public  Action<string> DisplayTimer;
    public static bool gameStarted;

    void OnEnable()
    {
        gameStarted = false;
    }
    void Update()
    {
        if (!_running) return;

        _remaining += Time.deltaTime;

        DisplayTimer?.Invoke(Display);

        if (_remaining >= duration)
        {
            _remaining = 0f;
            _running = false;
            OnComplete();
        }
    }

    public string Display => $"{Mathf.FloorToInt(_remaining / 60f):D2}:{Mathf.FloorToInt(_remaining % 60f):D2}";

    public void Pause() => _running = false;
    public void Resume() => _running = true;

    public void Begin(float time) { duration = time; _remaining = 0f; _running = true; }
    public void Stop() => _running = false;
    public void Reset() { _running = false; _remaining = duration; }

    private void OnComplete()
    {
        Debug.Log("Timer done!");
        // your logic here
        gameStarted = true;
        OnCompleteAct?.Invoke();
    }
}