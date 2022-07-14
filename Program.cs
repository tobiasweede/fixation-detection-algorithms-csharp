// See https://aka.ms/new-console-template for more information
using IDT;
using IVT;
using Gaze;
using System;
using System.IO;

Console.WriteLine("IDT Test Program");


string line;
int lineCount = 0;
string header;

Queue<GazeRecord> gazeRecordQueue = new Queue<GazeRecord>();

string path = "./data/";
string inFile = "et-2022-04-13-13-25.csv";
using (StreamReader reader = File.OpenText(path + inFile))
{
    while ((line = reader.ReadLine()) != null)
    {
        lineCount++;
        if (lineCount == 1)
        {
            header = line;
            // Frame;CaptureTime;LogTime;HMDPosition;HMDRotation;GazeStatus;CombinedGazeForward;CombinedGazePosition;InterPupillaryDistanceInMM;LeftEyeStatus;LeftEyeForward;LeftEyePosition;LeftPupilIrisDiameterRatio;LeftPupilDiameterInMM;LeftIrisDiameterInMM;RightEyeStatus;RightEyeForward;RightEyePosition;RightPupilIrisDiameterRatio;RightPupilDiameterInMM;RightIrisDiameterInMM;FocusDistance;FocusStability;FocusItem
            continue;
        }

        GazeRecord gazeRecord = new GazeRecord(line);
        gazeRecordQueue.Enqueue(gazeRecord);
        // if (lineCount > 3000) break;
    }
}


Ivt ivt = new Ivt();
ivt.ProcessRecordQueue(gazeRecordQueue);

string outFile = $"gazeEvents-{inFile.Substring(0, inFile.Length - 4)}.csv";
using (StreamWriter sw = new StreamWriter(path + outFile))
{
    sw.WriteLine("start;duration;type;");
    foreach (GazeEvent ge in ivt.eventList)
    {
        string result = $"{ge.start};{ge.duration};{ge.type}";
        Console.WriteLine(result);
        sw.WriteLine(result);
    }
}