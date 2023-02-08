namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;
    using VRC.Udon.Common.Interfaces;

    /// <summary>
    /// Megaphone script to make player voice louder
    /// </summary>
    [AddComponentMenu("OpsLib/VoiceAmplify")]
    public class VoiceAmplify : UdonSharpBehaviour
    {
        [SerializeField]
        private float FarDistance = 250.0f;
        [SerializeField]
        private float VoiceGain = 20.0f;

        private const float _defaultFarDist = 25.0f;
        private const float _defaultGain = 15.0f;

        public override void OnPickupUseDown()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "Shout");
        }

        public override void OnPickupUseUp()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, "Quiet");
        }

        /// <summary>
        /// Makes the player louder
        /// </summary>
        public void Shout()
        {
            var player = Networking.GetOwner(this.gameObject);
            if (player != null)
            {
                Debug.Log($"{player.displayName} is shouting!");
                player.SetVoiceDistanceFar(FarDistance);
                player.SetVoiceGain(VoiceGain);
                player.SetVoiceLowpass(false);
            }
        }

        /// <summary>
        /// Resets to default values
        /// </summary>
        public void Quiet()
        {
            var player = Networking.GetOwner(this.gameObject);
            if (player != null)
            {
                Debug.Log($"{player.displayName} is no longer shouting");
                player.SetVoiceDistanceFar(_defaultFarDist);
                player.SetVoiceGain(_defaultGain);
                player.SetVoiceLowpass(true);
            }
        }
    }
}