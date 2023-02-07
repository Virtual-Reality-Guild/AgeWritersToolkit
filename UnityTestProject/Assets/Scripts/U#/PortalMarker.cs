namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Wrapper around VRC_PortalMarker needed to set the RoomID from script
    /// </summary>
    [AddComponentMenu("OpsLib/PortalMarker")]
    public class PortalMarker : UdonSharpBehaviour
    {
        [SerializeField]
        private VRC_PortalMarker _portal;

        private void Start()
        {
            if (_portal == null)
                Debug.LogError("PortalMarker -- Cannot find VRC_PortalMarker component!");
        }

        public void SetPortalWorldId(string worldId)
        {
            if (_portal != null)
            {
                _portal.roomId = worldId;
            }
        }

        public void SetPortalOnOff(bool status)
        {
            _portal.gameObject.SetActive(status);
        }

        public void SetPortalOn()
        {
            _portal.gameObject.SetActive(true);
        }

        public void SetPortalOff()
        {
            _portal.gameObject.SetActive(false);
        }
    }
}