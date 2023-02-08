namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Makes a realtime light flicker
    /// </summary>
    [AddComponentMenu("OpsLib/LightFlicker")]
    public class LightFlicker : UdonSharpBehaviour
    {
        [SerializeField]
        private Light Target;

        [SerializeField]
        private float MaxIntensity = 1.0f;

        [SerializeField]
        private float MinIntensity = 0.0f;

        [SerializeField]
        private float Interval = 0.5f;

        [SerializeField]
        private bool isOn = false;

        private float _timerCounter = 0.0f;

        private void Update()
        {
            _timerCounter += Time.deltaTime;
            if (_timerCounter > Interval)
            {
                isOn = !isOn;
                if (isOn)
                    Target.intensity = MaxIntensity;
                else
                    Target.intensity = MinIntensity;

                _timerCounter = 0.0f;
            }
        }
    }
}