namespace OpsLib
{
    using System.Collections;
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Fades a light on or off - support for additional emissive materials and On/Off audio effects
    /// </summary>
    [AddComponentMenu("OpsLib/LightToggle")]
    public class LightToggle : UdonSharpBehaviour
    {
        [UdonSynced]
        public bool isOn = false;

        [SerializeField]
        [Tooltip("Fade length in seconds")]
        private float FadeLength = 5.0f;

        [SerializeField]
        private Material EmissiveMaterial;
        [SerializeField]
        private float EmissionValueMax = 100.0f;
        [SerializeField]
        private float EmissionValueMin = 0.0f;
        [SerializeField]
        private float EmissionIntensity = 0.0f;

        [SerializeField]
        private Light[] RTLights;
        [SerializeField]
        private float LightIntensityMin = 0.0f;
        [SerializeField]
        private float LightIntensityMax = 1.0f;

        [SerializeField]
        private AudioSource AudioSource;

        [SerializeField]
        private AudioClip AwakeSFX;

        [SerializeField]
        private AudioClip SleepSFX;

        private float _elapsedTime = 0.0f; //Time since the lights were last toggled
        private bool _resetState = false;
        private bool _isRunning = false;

        private void Start()
        {
            EmissiveMaterial.EnableKeyword("_EMISSION");
            _elapsedTime = FadeLength + 1; // Make sure we do nothing when we first start
        }

        public void ToggleLights()
        {
            // Do the toggle for the local client
            DoToggle();
        }

        public void PulseLights()
        {
            //Turn lights on and then off automatically
            DoToggle();
            //Set bool to reset the state back to the beginning
            _resetState = true;
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            //Resync players when a new one joins
            if (player.isLocal)
            {
                RequestSerialization();
            }
        }

        public override void OnDeserialization()
        {
            //Check to make sure we aren't already doing an animation to avoid popping
            if (!_isRunning)
                Resync();
        }

        private void Resync()
        {
            Color emissionValue;
            if (isOn)
            {
                foreach (var light in RTLights)
                {
                    light.intensity = LightIntensityMax;
                }
                emissionValue = Color.HSVToRGB(0, 0, EmissionValueMax);
            }
            else
            {
                foreach (var light in RTLights)
                {
                    light.intensity = LightIntensityMin;
                }
                emissionValue = Color.HSVToRGB(0, 0, EmissionValueMin);
            }
            EmissiveMaterial.SetColor("_EmissionColor", emissionValue * EmissionIntensity);
        }

        private void DoToggle()
        {
            // Prevent players from spamming the button
            if (!_isRunning)
            {
                isOn = !isOn;                           //Toggle value
                _elapsedTime = 0.0f;                    //Set lights to do fade in/out
                _isRunning = true;                      //Set _isRunning
                PlaySound();                            //Play sound
                Debug.Log($"Lights set to {isOn}");
            }
        }

        private void Update()
        {
            if (_elapsedTime < FadeLength)
            {
                _elapsedTime += Time.deltaTime;
                UpdateEmission();
                UpdateLights();
            }
            else if (_resetState)
            {
                //After we've finished our fade reset back
                _resetState = false;
                _isRunning = false;
                DoToggle();
            }
            else
            {
                //We've finished all we had to do
                _isRunning = false;
            }
        }

        private void UpdateEmission()
        {
            if (EmissiveMaterial != null)
            {
                Color emissionValue;
                if (isOn)
                {
                    //Turning lights on
                    emissionValue = Color.HSVToRGB(0, 0, Mathf.Lerp(EmissionValueMin, EmissionValueMax, _elapsedTime / FadeLength));
                }
                else
                {
                    //Turning lights off
                    emissionValue = Color.HSVToRGB(0, 0, Mathf.Lerp(EmissionValueMax, EmissionValueMin, _elapsedTime / FadeLength));
                }

                EmissiveMaterial.SetColor("_EmissionColor", emissionValue * EmissionIntensity);
            }
        }

        private void UpdateLights()
        {
            if (RTLights.Length > 0)
            {
                foreach (var light in RTLights)
                {
                    if (light != null)
                    {
                        if (isOn)
                        {
                            //Turning lights on
                            light.intensity = Mathf.Lerp(LightIntensityMin, LightIntensityMax, _elapsedTime / FadeLength);
                        }
                        else
                        {
                            //Turning lights off
                            light.intensity = Mathf.Lerp(LightIntensityMax, LightIntensityMin, _elapsedTime / FadeLength);
                        }
                    }
                }
            }
        }

        private void PlaySound()
        {
            if (isOn)
            {
                if (AwakeSFX != null)
                {
                    AudioSource.clip = AwakeSFX;
                    AudioSource.Play();
                }
            }
            else
            {
                if (SleepSFX != null)
                {
                    AudioSource.clip = SleepSFX;
                    AudioSource.Play();
                }
            }
        }
    }
}