namespace OpsLib
{
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Adjusts a seat up or down with support for syncing the adjusted seat position
    /// </summary>
    [AddComponentMenu("OpsLib/SeatAdjust")]
    public class SeatAdjust : UdonSharpBehaviour
    {
        [SerializeField]
        private float SeatMaxHeight;
        [SerializeField]
        private float SeatMinHeight;
        [SerializeField]
        private float HeightAdjInterval;
        [SerializeField]
        private GameObject target;

        [UdonSynced]
        private float SeatHeight;

        void Start()
        {
            SeatHeight = target.gameObject.transform.localPosition.y;
        }

        public override void OnDeserialization()
        {
            SetSeatHeight();
        }

        public void RaiseSeat()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);

            if (SeatHeight + HeightAdjInterval <= SeatMaxHeight)
            {
                SeatHeight += HeightAdjInterval;
                SetSeatHeight();
                RequestSerialization();
            }
        }

        public void LowerSeat()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);

            if (SeatHeight - HeightAdjInterval >= SeatMinHeight)
            {
                SeatHeight -= HeightAdjInterval;
                SetSeatHeight();
                RequestSerialization();
            }
        }

        private void SetSeatHeight()
        {
            var localPosition = target.transform.localPosition;
            target.transform.localPosition = new Vector3(localPosition.x, SeatHeight, localPosition.z);
        }
    }
}