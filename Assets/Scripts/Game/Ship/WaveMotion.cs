using System;
using System.Collections;
using Ferry.Config;
using Ferry_boat.Assets.Scripts.Web;
using GF;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ferry.Motion
{
    public class WaveMotion : MonoBehaviour
    {
        [Header("Boat Visual Bobbing")]
        [SerializeField] Transform boatVisual;

        private Vector3 visualStartLocalPos;
        private Quaternion visualStartLocalRot;
        private bool IsInitialized = false;

        public WaveConfig waveConfig;
        private DifficultyLevel targetLevel;
        private DifficultyLevel startLevel = DifficultyLevel.easy;

        // live values
        private float waveAmplitude;
        private float waveSpeed;
        private float rollAmount;
        private float pitchAmount;

        // FROM snapshot
        private float fromAmplitude, fromSpeed, fromRoll, fromPitch;

        // TO target
        private float toAmplitude, toSpeed, toRoll, toPitch;

        public bool IsTransitioning;
        private float currentSmoothTime;
        private const float maxSmoothTime =20f;

        // phase accumulator — eliminates sine jump when speed changes
        private float wavePhase = 0f;
        [Header("Wave difficulty")]
        public float currentWindSpeed;
        public float currentWaveLevel;

        void OnEnable()
        {
            if (boatVisual != null)
            {
                visualStartLocalPos = boatVisual.localPosition;
                visualStartLocalRot = boatVisual.localRotation;
                
            }
        }

        IEnumerator CurrentWaveDifficulty(DifficultyLevel level)
        {
        
                yield return new WaitForSecondsRealtime(5f);
                currentWindSpeed = Wave_Difficulty.windspeed;
                currentWaveLevel = Wave_Difficulty.wavelevel;
                Debug.LogWarning($"Wavelevel {currentWaveLevel} | windspeed {currentWindSpeed} ");
            
        }

        void OnDisable()
        {
            IsInitialized   = false;
            IsTransitioning = false;
        }

        public void SetLevel(DifficultyLevel level)
        {
            targetLevel   = level;
            var data      = waveConfig.WaveDatas[(int)startLevel];
            waveAmplitude = data.waveHeight;
            waveSpeed     = data.waveSpeed;
            rollAmount    = data.rollAmount;
            pitchAmount   = data.pitchAmount;

            wavePhase       = 0f;
            IsInitialized   = true;
            IsTransitioning = false;
            StartCoroutine(CurrentWaveDifficulty(level));
        }

        public void SetTargetLevel()
        {
            var data = waveConfig.WaveDatas[(int)targetLevel];
            BeginTransition(data.waveHeight, data.waveSpeed,
                            data.rollAmount, data.pitchAmount);
        }

        internal void ResetLevel()
        {
            var data = waveConfig.WaveDatas[(int)startLevel];
            BeginTransition(data.waveHeight, data.waveSpeed,
                            data.rollAmount, data.pitchAmount);
        }

        void Update()
        {
            if (!IsInitialized) return;

            if (IsTransitioning)
                TickTransition();

            ApplyWaveMotion();
        }

        private void BeginTransition(float toAmp, float toSpd,
                                     float toRol, float toPit)
        {
            fromAmplitude = waveAmplitude;
            fromSpeed     = waveSpeed;
            fromRoll      = rollAmount;
            fromPitch     = pitchAmount;

            toAmplitude = toAmp;
            toSpeed     = toSpd;
            toRoll      = toRol;
            toPitch     = toPit;

            currentSmoothTime = 0f;
            IsTransitioning   = true;
            IsInitialized     = true;
        }

        private void TickTransition()
        {
            currentSmoothTime += Time.deltaTime;

            float t      = Mathf.Clamp01(currentSmoothTime / maxSmoothTime);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            waveAmplitude = Mathf.Lerp(fromAmplitude, toAmplitude, smooth);
            rollAmount    = Mathf.Lerp(fromRoll,      toRoll,      smooth);
            pitchAmount   = Mathf.Lerp(fromPitch,     toPitch,     smooth);

            // speed lerps much slower to avoid phase jumps
            waveSpeed = Mathf.Lerp(fromSpeed, toSpeed, smooth * 0.3f);

            if (t >= 1f)
            {
                waveSpeed       = toSpeed;  // snap cleanly at end
                IsTransitioning = false;
            }
        }

        private void ApplyWaveMotion()
        {
            if (boatVisual == null) return;

            // advance phase by deltaTime instead of using Time.time * speed
            // keeps the sine wave continuous even when speed changes
            wavePhase += Time.deltaTime * waveSpeed;
            float waveFactor = Mathf.InverseLerp(3f,11f,currentWaveLevel);
            float windfactor = Mathf.InverseLerp(5f,35f,currentWindSpeed);
            float seaFactor = Mathf.Lerp(1f,3f,(waveFactor + windfactor)*0.5f);

            float bob   = Mathf.Sin(wavePhase)        * waveAmplitude;
            float roll  = Mathf.Sin(wavePhase * 1.3f) * rollAmount;
            float pitch = Mathf.Cos(wavePhase * 1.1f) * pitchAmount;
            roll *= seaFactor;
            pitch *=seaFactor;
            bob *= seaFactor;

            boatVisual.localPosition = visualStartLocalPos + new Vector3(0f, bob, 0f);
            boatVisual.localRotation = visualStartLocalRot * Quaternion.Euler(pitch, 0f, roll);
        }
    }
}