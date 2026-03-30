using System;
using UnityEngine;

namespace Ferry.Config
{
    [CreateAssetMenu(fileName = "WaveConfig", menuName = "Config/WaveConfig")]
    public class WaveConfig : ScriptableObject
    {
        public WaveData[] WaveDatas;
    }
    [Serializable]
    public class WaveData
    {
        public float waveHeight = 0.03f;
        public float waveSpeed = 0.8f;
        public float rollAmount = 1.5f;
        public float pitchAmount = 1.0f;
    }
}