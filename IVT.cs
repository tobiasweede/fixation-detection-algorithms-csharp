using Gaze;
namespace IVT
{
    public class Ivt
    {
        private float minBlinkDurationThreshold = 0.05f; // 1 Sec / 200 Hz = 0.005 --> 0.05 sec TH means min. 10 invalid samples
        private float minFixationDurationThreshold = 0.1f; // 100ms (see Homquist)
        private float velocityThreshold = 50.0f; // 50°/s (see Holmquist p. 231)
        GazeEvent? currentGazeEvent = null;
        public List<GazeEvent> eventList = new List<GazeEvent>();
        GazeRecord lastGazeRecord = new GazeRecord();

        public void ProcessRecordQueue(Queue<GazeRecord> gazeRecordQueue)
        {
            while (gazeRecordQueue.Count > 0)
            {
                GazeRecord currentGaze = gazeRecordQueue.Dequeue();

                ProcessGazeEvent(currentGaze);
            }
        }

        void ProcessGazeEvent(GazeRecord currentGazeRecord)
        {
            if (currentGazeEvent == null)
            { // Create new event object on start 
                currentGazeEvent = new GazeEvent(currentGazeRecord.captureTime, EventType.Init);
                currentGazeEvent.start = currentGazeRecord.captureTime;
                lastGazeRecord = currentGazeRecord;
                return;
            }

            float timeDeltaInSec = GazeFunctions.TimeDeltaInSec(lastGazeRecord, currentGazeRecord);
            currentGazeEvent.duration += timeDeltaInSec;

            if (lastGazeRecord.valid)
            {
                if (currentGazeRecord.valid)
                {
                    float velocity = GazeFunctions.GetAngularVelocity(lastGazeRecord, currentGazeRecord, timeDeltaInSec);
                    if (velocity > velocityThreshold)
                    { // Saccade
                        if (currentGazeEvent.eventType == EventType.Init)
                        {
                            currentGazeEvent.start = currentGazeRecord.captureTime;
                            currentGazeEvent.duration = timeDeltaInSec;
                            currentGazeEvent.eventType = EventType.Saccade;
                        }
                        else if (currentGazeEvent.eventType == EventType.Undefined)
                        {
                            currentGazeEvent.eventType = EventType.Saccade;
                        }
                        if (currentGazeEvent.eventType != EventType.Saccade)
                        {
                            AddEvent(currentGazeEvent);
                            currentGazeEvent = new GazeEvent(currentGazeRecord.captureTime, EventType.Saccade);
                        }
                        currentGazeEvent.velocityList.Add(velocity);
                    }
                    else
                    { // Fixation
                        if (currentGazeEvent.eventType == EventType.Init)
                        {
                            currentGazeEvent.start = currentGazeRecord.captureTime;
                            currentGazeEvent.duration = timeDeltaInSec;
                            currentGazeEvent.eventType = EventType.Fixation;
                        }
                        else if (currentGazeEvent.eventType == EventType.Undefined)
                        {
                            currentGazeEvent.eventType = EventType.Fixation;
                        }
                        if (currentGazeEvent.eventType != EventType.Fixation)
                        {
                            AddEvent(currentGazeEvent);
                            currentGazeEvent = new GazeEvent(currentGazeRecord.captureTime, EventType.Fixation);
                        }
                    }
                }
                else
                {
                    // last valid and current invalid
                    // begin of a blink 
                    if (currentGazeEvent.eventType != EventType.Init && currentGazeEvent.eventType != EventType.Undefined)
                        AddEvent(currentGazeEvent);
                    currentGazeEvent = new GazeEvent(currentGazeRecord.captureTime, EventType.Undefined);
                }
            }
            else
            {  // lastGaze invalid
                if (currentGazeRecord.valid)
                {
                    if (currentGazeEvent.eventType != EventType.Init)
                    {
                        if (currentGazeEvent.duration > minBlinkDurationThreshold)
                        {
                            currentGazeEvent.eventType = EventType.Blink;
                            eventList.Add(currentGazeEvent);
                        }
                        currentGazeEvent = new GazeEvent(currentGazeRecord.captureTime, EventType.Undefined); // type unknown
                    }
                }
            }
            lastGazeRecord = currentGazeRecord;
        }

        void AddEvent(GazeEvent currentGazeEvent)
        {
            if (currentGazeEvent.eventType == EventType.Fixation && currentGazeEvent.duration < minFixationDurationThreshold)
                return;
            if (currentGazeEvent.eventType == EventType.Saccade)
                currentGazeEvent.velocity = currentGazeEvent.velocityList.Average();
            eventList.Add(currentGazeEvent);
        }

    }
}