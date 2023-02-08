namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;
    using TMPro;

    /// <summary>
    /// Figures out who the instance master is and displays it on a TMP Text field
    /// </summary>
    [AddComponentMenu("OpsLib/MasterDisplay")]
    public class MasterDisplay : UdonSharpBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI TextField;

        private void Start()
        {
            FindMaster();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            FindMaster();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            FindMaster();
        }

        private void FindMaster()
        {
            if (TextField != null)
            {
                VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
                foreach (var player in VRCPlayerApi.GetPlayers(players))
                {
                    if (player.isMaster)
                    {
                        TextField.text = $"Master: {player.displayName}";
                        break;
                    }
                }
            }
        }
    }
}