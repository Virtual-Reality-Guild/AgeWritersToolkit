namespace OpsLib
{
    using System;
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Toggles gameObjects on for a given Holiday period
    /// </summary>
    [AddComponentMenu("OpsLib/HolidayObjectToggles")]
    public class HolidayObjectToggles : UdonSharpBehaviour
    {
        //Starting day of the holiday period
        public int StartMonth = 1;
        public int StartDay = 1;

        public int Duration; //In Days

        public GameObject[] HolidayObjects; //Toggled on during the holiday period

        private DateTime _holidayStart;

        void Start()
        {
            _holidayStart = new DateTime(DateTime.Now.Year, StartMonth, StartDay);

            var now = DateTime.Now;
            Debug.Log($"It is: {now.Date}");
            Debug.Log($"The holiday is on {_holidayStart.Date} for {Duration} days");

            if (now >= _holidayStart && now <= _holidayStart.AddDays(Duration))
            {
                Debug.Log("It is holiday time!");
                foreach (GameObject go in HolidayObjects)
                {
                    go.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject go in HolidayObjects)
                    go.SetActive(false);
            }
        }
    }
}