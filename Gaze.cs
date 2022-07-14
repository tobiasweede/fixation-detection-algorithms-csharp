using System.Numerics;
using System.Text.RegularExpressions;
using System;

namespace Gaze
{
    class GazeFunctions
    {
        public static float TimeDeltaInSec(GazeRecord firstRecord, GazeRecord secondRecord)
        {
            // Convert to Second 1sec = 1E9 Nanoseconds
            return (float)((secondRecord.captureTime - firstRecord.captureTime) / 1E9f);
        }
        public static float GetAngularVelocity(GazeRecord firstRecord, GazeRecord secondRecord, float timeDeltaInSec)
        {

            // Replace in unity by Vector3.Angle function
            float velocity = MathF.Acos(
                Vector3.Dot(firstRecord.gazeDirection, secondRecord.gazeDirection) /
                (firstRecord.gazeDirection.Length() * secondRecord.gazeDirection.Length())) /
                timeDeltaInSec;

            return velocity;
        }
    }
    public enum EventType
    {
        Init,
        Blink,
        Fixation,
        Saccade
    }
    public class GazeEvent
    {
        public EventType type;
        public long start;
        public float duration;
        public float velocity;
        public GazeEvent()
        {
            type = EventType.Init;
        }
        public GazeEvent(long _start, EventType _type)
        {
            start = _start;
            type = _type;
        }
    }

    public class GazeRecord
    {
        public long captureTime;
        public bool valid;
        public Vector3 hmdPosition;
        public Vector3 gazeDirection;

        public GazeRecord()
        {
            valid = false;
        }

        public GazeRecord(string csvString)
        {
            // Regex for (-X, X, X) vector strings
            Regex rx = new Regex(@"\((?<X>-?\d+.\d+), (?<Y>-?\d+.\d+), (?<Z>-?\d+.\d+)\)");
            Match match;
            GroupCollection group;

            string[] items = csvString.Split(';');

            captureTime = long.Parse(items[1]);

            string gazeStatusString = items[5];
            if (gazeStatusString.Equals("VALID"))
            {

                valid = true;

                // head position
                string hmdPositionString = items[3];
                match = rx.Match(hmdPositionString);
                group = match.Groups;
                float hmdX = float.Parse(group["X"].Value);
                float hmdY = float.Parse(group["Y"].Value);
                float hmdZ = float.Parse(group["Z"].Value);
                hmdPosition = new Vector3(hmdX, hmdY, hmdZ);

                // gaze direction 
                string combinedGazeForwardString = items[6];
                match = rx.Match(combinedGazeForwardString);
                group = match.Groups;
                float gazeX = float.Parse(group["X"].Value);
                float gazeY = float.Parse(group["Y"].Value);
                float gazeZ = float.Parse(group["Z"].Value);
                gazeDirection = new Vector3(gazeX, gazeY, gazeZ);

                // Console.WriteLine($"Time: {caputureTime.ToString()} Status: {gazeStatus.ToString()} " +
                // $"HeadPos: {hmdX.ToString()} {hmdY.ToString()} {hmdZ.ToString()} " +
                // $"GazeDir: {gazeX.ToString()} {gazeY.ToString()} {gazeZ.ToString()}");
            }

        }
    }
}