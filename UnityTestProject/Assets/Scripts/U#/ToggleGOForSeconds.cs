namespace OpsLib 
{     
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Toggles a set of game objects for a given amount of seconds
    /// </summary>
    [AddComponentMenu("OpsLib/ToggleGOForSeconds")]
    public class ToggleGOForSeconds : UdonSharpBehaviour
    {
        [SerializeField]
        private GameObject[] targets;

        [SerializeField]
        private float interval = 1.0f;

        private float _timerCounter = 0.0f;
        private bool _doToggle;

        public void DoToggle()
        {
            //Toggle the GOs
            foreach (GameObject target in targets)
            {
                Debug.Log($"Toggling {target.name}");
                target.SetActive(!target.activeSelf);
            }

            //Start the counter
            _doToggle = true;
        }

        private void Update()
        {
            if (_doToggle)
            {
                _timerCounter += Time.deltaTime;
                if(_timerCounter > interval)
                {
                    //Toggle the GOs
                    foreach (GameObject target in targets)
                    {
                        target.SetActive(!target.activeSelf);
                    }

                    //Stop the counter
                    _timerCounter = 0.0f;
                    _doToggle = false;
                }
            }
        }
    }
}