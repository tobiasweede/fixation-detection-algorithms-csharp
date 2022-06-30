// See https://aka.ms/new-console-template for more information
using IDT;
using System;
using System.IO;

Console.WriteLine("IDT Test Program");

// Console.Log("Debug output");
Console.WriteLine("Test");

Fixation.TestStaticMethod();
Fixation f = new Fixation();
f.TestMethod();

string path = "./data/";
string file = "et-2022-04-13-13-25.csv";

StreamReader reader = File.OpenText(path + file);
string line;
int lineCount = 0;
string header;
while ((line = reader.ReadLine()) != null)
{
    lineCount++;
    if (lineCount == 1)
    {
        header = line;
        continue;
    }

    // Frame;CaptureTime;LogTime;HMDPosition;HMDRotation;GazeStatus;CombinedGazeForward;CombinedGazePosition;InterPupillaryDistanceInMM;LeftEyeStatus;LeftEyeForward;LeftEyePosition;LeftPupilIrisDiameterRatio;LeftPupilDiameterInMM;LeftIrisDiameterInMM;RightEyeStatus;RightEyeForward;RightEyePosition;RightPupilIrisDiameterRatio;RightPupilDiameterInMM;RightIrisDiameterInMM;FocusDistance;FocusStability;FocusItem
    string[] items = line.Split(';');
    long Frame = long.Parse(items[0]);
    long CaputureTime = long.Parse(items[1]);
    long LogTime = long.Parse(items[2]);
    string HMDPosition = items[3];

    Console.WriteLine(Frame.ToString() + " " + CaputureTime.ToString() + " " + HMDPosition);
    if (lineCount == 10) break;
}