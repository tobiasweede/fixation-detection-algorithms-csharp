using Gaze;
namespace IVT
{
    public class Ivt
    {
        private float velocityThreshold = 30.0f; // 50°/s (see Holmquist p. 231)
        GazeEvent? currentGazeEvent = null;
        public List<GazeEvent> eventList = new List<GazeEvent>();
        GazeRecord lastGaze = new GazeRecord();

        public void ProcessRecordQueue(Queue<GazeRecord> gazeRecordQueue)
        {
            while (gazeRecordQueue.Count > 0)
            {
                GazeRecord currentGaze = gazeRecordQueue.Dequeue();

                ProcessGazeEvent(currentGaze);
            }
        }

        void ProcessGazeEvent(GazeRecord currentGaze)
        {
            if (currentGazeEvent == null)
            { // New event
                currentGazeEvent = new GazeEvent(currentGaze.captureTime, EventType.Init);
                currentGazeEvent.start = currentGaze.captureTime;
                lastGaze = currentGaze;
                return;
            }

            float timeDeltaInSec = GazeFunctions.TimeDeltaInSec(lastGaze, currentGaze);
            currentGazeEvent.duration += timeDeltaInSec;

            if (lastGaze.valid)
            {
                if (currentGaze.valid)
                {
                    float velocity = GazeFunctions.GetAngularVelocity(lastGaze, currentGaze, timeDeltaInSec);
                    if (velocity > velocityThreshold)
                    { // Saccade
                        if (currentGazeEvent.type == EventType.Init)
                        {
                            currentGazeEvent.type = EventType.Saccade;
                        }
                        if (currentGazeEvent.type != EventType.Saccade)
                        {
                            eventList.Add(currentGazeEvent);
                            currentGazeEvent = new GazeEvent(currentGaze.captureTime, EventType.Saccade);
                            currentGazeEvent.velocity = velocity;
                        }
                    }
                    else
                    { // Fixation
                        if (currentGazeEvent.type == EventType.Init)
                        {
                            currentGazeEvent.type = EventType.Fixation;
                        }
                        if (currentGazeEvent.type != EventType.Fixation)
                        {
                            eventList.Add(currentGazeEvent);
                            currentGazeEvent = new GazeEvent(currentGaze.captureTime, EventType.Fixation);
                        }
                    }

                }
                else
                { // currentGaze invalid
                    if (currentGazeEvent.type != EventType.Init)
                    {
                        eventList.Add(currentGazeEvent);
                        currentGazeEvent = new GazeEvent(currentGaze.captureTime, EventType.Blink);
                    }
                }
            }
            else
            {  // lastGaze invalid
                if (currentGaze.valid)
                {
                    if (currentGazeEvent.type != EventType.Init)
                    {
                        currentGazeEvent.type = EventType.Blink;
                        eventList.Add(currentGazeEvent);
                        currentGazeEvent = new GazeEvent(currentGaze.captureTime, EventType.Init);
                    }
                }
            }
            lastGaze = currentGaze;
        }

    }
}