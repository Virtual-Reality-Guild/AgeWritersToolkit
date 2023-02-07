namespace OpsLib
{
    using System;
    using UdonSharp;
    using UnityEngine;
    using VRC.SDKBase;
    using VRC.Udon;

    /// <summary>
    /// Converts UTC time to DniTime and offsets the materials of the D'ni clock
    /// </summary>
    [AddComponentMenu("OpsLib/DniTimeClock")]
    [ExecuteInEditMode]
    public class DniTimeClock : UdonSharpBehaviour
    {
        /// <summary>
        /// The clock materials we will offset
        /// </summary>
        public Material[] Materials;

        //The xOffset value for the materials
        private float _xOffset = -0.04f;    //The texture at 0 actually reads 1, so we need to run the clock back by one pahrtahvo

        #region Jehon's D'ni Clock Script
        // ***** D'ni Clock Script *****
        // Original Author: Jehon the Scribe (mail: Jehon[at]gmx.net)
        // Ported from JS to C# and Modified By: Opcode_ (discord: Opcode_#1542)
        // Last Update: 2022-09-13; additions by rokama: 2015-04-24
        // If you use this code somewhere, please give proper attribution.
        // A quick note on timing: this script uses UTC throughout. But not quite. It relies on
        // .NET's DateTimeOffset.ToUnixTimeMilliseconds() and DateTimeOffset.UtcNow functions, both of which return the number of seconds that have elapsed since 1970-01-01T00:00:00Z (January 1, 1970, at 12:00 AM UTC).
        // It does not take leap seconds into account. This method returns the number of milliseconds in Unix time (https://en.wikipedia.org/wiki/Unix_time),
        // so it's not precisely UTC. This script tries to adjust for leap seconds, for better accuracy.

        // list of leap second time stamps from 1972-01-01 to 2015-07-01
        // source: https://www.ietf.org/timezones/data/leap-seconds.list (adjusted to Unix Time)
        private long[] LeapSecTimeStamps = { 63072000000, 78796800000, 94694400000, 126230400000, 157766400000, 189302400000, 220924800000, 252460800000, 283996800000, 315532800000, 362793600000, 394329600000, 425865600000, 489024000000, 567993600000, 631152000000, 662688000000, 709948800000, 741484800000, 773020800000, 820454400000, 867715200000, 915148800000, 1136073600000, 1230768000000, 1341100800000, 1435708800000 };
        private const int LeapSecOffset = 10;   // 1972-01-01 started with 10 leap seconds

        // reference date: 9647 leefo 1 0.0.0.0 = 1991-04-21 16:54:00 UTC
        // (time stamp on the original HyperCard Stack file for MYST) - source: RAWA, RIUM+
        private long RefMillisec = new DateTimeOffset(1991, 04, 21, 16, 54, 00, TimeSpan.Zero).ToUnixTimeMilliseconds();
        private const long RefHar = 9647;
        private const long MillisecPerHar = 31556925216;    // Mean Solar Tropical Year in 1995, source: RAWA
        private const long ProranteePerHar = 10 * 29 * 5 * 25 * 25 * 25;       // = 22656250
        private long har;
        private long vilee;
        private long yar, gartavo, partavo, tavoMod5, tavo, goran, proran;

        private long AdjustForLeapSeconds(long TimeStamp)
        {
            var leapSecs = 0;
            var arrayLen = LeapSecTimeStamps.Length;
            for (var i = 0; i < arrayLen && TimeStamp >= LeapSecTimeStamps[i]; leapSecs++, i++) ;
            if (leapSecs > 0) leapSecs += LeapSecOffset;
            return TimeStamp + leapSecs * 1000;
        }

        private long MakeTimeStamp(int year, int month, int day, int hour, int minute, int second)
        {
            DateTimeOffset date = new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero);
            long temp = date.ToUnixTimeMilliseconds();
            return AdjustForLeapSeconds(temp);
        }

        private void Earth2Dni(long UnixTimeStamp)
        {
            var delta = AdjustForLeapSeconds(UnixTimeStamp) - AdjustForLeapSeconds(RefMillisec);

            har = Mathf.FloorToInt(delta / MillisecPerHar);
            delta -= (har * MillisecPerHar);
            var delta2 = (double)delta * ((double)ProranteePerHar / (double)MillisecPerHar);
            vilee = Mathf.FloorToInt((float)(delta2 / (29 * 5 * 25 * 25 * 25)));
            delta2 -= vilee * (29 * 5 * 25 * 25 * 25);
            yar = Mathf.FloorToInt((float)delta2 / (5 * 25 * 25 * 25));
            delta2 -= yar * (5 * 25 * 25 * 25);
            gartavo = Mathf.FloorToInt((float)delta2 / (25 * 25 * 25));
            delta2 -= gartavo * (25 * 25 * 25);
            tavo = Mathf.FloorToInt((float)delta2 / (25 * 25));
            delta2 -= tavo * (25 * 25);
            goran = Mathf.FloorToInt((float)delta2 / 25);
            delta2 -= goran * 25;
            proran = Mathf.FloorToInt((float)delta2);
            // correct potential value underflow (values before reference date)
            if (proran < 0) { proran += 25; goran--; }
            if (goran < 0) { goran += 25; tavo--; }
            if (tavo < 0) { tavo += 25; gartavo--; }
            if (gartavo < 0) { gartavo += 5; yar--; }
            if (yar < 0) { yar += 29; vilee--; }
            if (vilee < 0) { vilee += 10; har--; }
            // calculate pahrtahvo from gahrtahvo and tahvo
            partavo = gartavo * 5 + Mathf.FloorToInt(tavo / 5);
            Math.DivRem(tavo, 5, out tavoMod5);
            // add reference D'ni hahr (year) and make vilee (month) & yahr (day) 1-based instead of 0-based
            har += RefHar;
            vilee++;
            yar++;

            //Debug.Log($"D'ni Time: {har}.{vilee}.{yar} - {partavo}.{tavoMod5}.{goran}.{proran}");
        }
        #endregion

        void FixedUpdate()
        {
            //Get the current time
            var D = DateTimeOffset.UtcNow;
            var TimeStamp = D.ToUnixTimeMilliseconds();
            //Debug.Log($"Earth Time: {D}");
            //Debug.Log($"Unix Time: {TimeStamp}");
            Earth2Dni(TimeStamp);

            //Set the initial rotation
            _xOffset = -0.04f;  //The texture at 0 actually reads 1, so we need to run the clock back by one pahrtahvo

            //Assuming a full rotation is 0-1 texture offset, then each pahrtahvo is 1/25 or 0.04. Mult by how many partavo there are
            _xOffset += partavo * 0.04f;

            //Offset for each tahvotee is 1/5 of a partavo or 0.008
            _xOffset += tavoMod5 * 0.008f;

            //Offset for each gorahn is 1/25 of a tahvo or 0.00032
            _xOffset += goran * 0.00032f;

            //Offset for each prorahn is 1/25 pf a goran or 0.0000128
            _xOffset += proran * 0.0000128f;

            foreach (var material in Materials)
            {
                material.mainTextureOffset = new Vector2(_xOffset, 0.0f);
            }
        }
    }
}