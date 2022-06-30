namespace IDT
{
    public class Fixation
    {
        public static void TestStaticMethod()
        {
            Console.WriteLine("Test static method!");
        }

        public void TestMethod()
        {
            Console.WriteLine("Test method!");
        }

    }
    
    // Frame;CaptureTime;LogTime;HMDPosition;HMDRotation;GazeStatus;CombinedGazeForward;CombinedGazePosition;InterPupillaryDistanceInMM;LeftEyeStatus;LeftEyeForward;LeftEyePosition;LeftPupilIrisDiameterRatio;LeftPupilDiameterInMM;LeftIrisDiameterInMM;RightEyeStatus;RightEyeForward;RightEyePosition;RightPupilIrisDiameterRatio;RightPupilDiameterInMM;RightIrisDiameterInMM;FocusDistance;FocusStability;FocusItem
    public class Record
    {
        public long Frame;

        public long CaptureTime;
    }

}
