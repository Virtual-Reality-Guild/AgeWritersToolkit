namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;
    
    /// <summary>
    /// Script for triggering animations on a door for open/close and playing sound
    /// </summary>
    [AddComponentMenu("OpsLib/ToggleAnims")]
    public class ToggleAnims : UdonSharpBehaviour
    {
        /// <summary>
        /// List of AnimationControllers to trigger
        /// </summary>
        [SerializeField]
        private Animator[] Animators;

        /// <summary>
        /// Name of the bool in the animation controller corresponding to door state - must be the same across all animators!
        /// </summary>
        [SerializeField]
        private string AnimatorBoolName = "isOpen";

        /// <summary>
        /// Optional GameObjects that toggle on/off for a specified amount of time while the opening animation is playing
        /// </summary>
        [SerializeField]
        private ToggleGOForSeconds OpeningAnimationToggleableGOs;

        /// <summary>
        /// Optional GameObjects that toggle on/off for a specified amount of time while the closing animation is playing
        /// </summary>
        [SerializeField]
        private ToggleGOForSeconds ClosingAnimationToggleableGOs;

        /// <summary>
        /// Optional GameObjects to toggle when the door changes state
        /// </summary>
        [SerializeField]
        private GameObject[] ToggleGameObjects;

        /// <summary>
        /// Optional Door open sound
        /// </summary>
        [SerializeField]
        private AudioSource OpeningAudio;

        /// <summary>
        /// Optional Door close sound
        /// </summary>
        [SerializeField]
        private AudioSource ClosingAudio;

        /// <summary>
        /// Optional opening text
        /// </summary>
        [SerializeField]
        private string OpeningText;

        /// <summary>
        /// Optional closing text
        /// </summary>
        [SerializeField]
        private string ClosingText;

        /// <summary>
        /// Optional Text controller to set a text field when the animation is playing
        /// </summary>
        [SerializeField]
        private TextController TextController;

        /// <summary>
        /// Set this to your door's default state on awake - synced over the network
        /// </summary>
        [SerializeField]
        [UdonSynced]
        private bool isOpen;

        /// <summary>
        /// Toggle Door between open/closed
        /// </summary>
        public void ToggleDoor()
        {
            //Make sure we have authority to make the change
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);

            if (isOpen)
            {
                Close();
            }
            else if (!isOpen)
            {
                Open();
            }

            isOpen = !isOpen;
            RequestSerialization();
        }

        /// <summary>
        /// Just open the door
        /// </summary>
        public void OpenDoor()
        {
            if (!isOpen)
            {
                //Make sure we have authority to make the change
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);

                Open();
                isOpen = true;
                RequestSerialization();
            }
        }

        /// <summary>
        /// Just close the door
        /// </summary>
        public void CloseDoor()
        {
            if (isOpen)
            {
                //Make sure we have authority to make the change
                Networking.SetOwner(Networking.LocalPlayer, this.gameObject);

                Close();
                isOpen = false;
                RequestSerialization();
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            //Resync when a new player joins
            if (player.isLocal)
            {
                RequestSerialization();
            }
        }

        public override void OnDeserialization()
        {
            Resync();
        }

        private void Resync()
        {
            foreach (var anim in Animators)
            {
                if (anim.GetBool(AnimatorBoolName) != isOpen)
                {
                    Debug.LogWarning($"{this.gameObject.name} -- Resync needed!");
                    if (isOpen)
                        Open();
                    else
                        Close();
                }
            }
        }

        private void Open()
        {
            Debug.Log("Opening door!");
            foreach (var anim in Animators)            
                anim.SetBool(AnimatorBoolName, true);
            
            foreach (var go in ToggleGameObjects)
                go.SetActive(!go.activeSelf);

            if (OpeningAnimationToggleableGOs != null)
                OpeningAnimationToggleableGOs.DoToggle();

            if (TextController != null && !string.IsNullOrEmpty(OpeningText))
            {
                TextController.SetText(OpeningText);
            }

            if (OpeningAudio != null)
                OpeningAudio.Play();
        }

        private void Close()
        {
            Debug.Log("Closing door!");
            foreach (var anim in Animators)
                anim.SetBool(AnimatorBoolName, false);

            foreach (var go in ToggleGameObjects)
                go.SetActive(!go.activeSelf);

            if(ClosingAnimationToggleableGOs != null)
                ClosingAnimationToggleableGOs.DoToggle();

            if (TextController != null && !string.IsNullOrEmpty(ClosingText))
            {
                TextController.SetText(ClosingText);
            }

            if (ClosingAudio != null)
                ClosingAudio.Play();
        }
    }
}