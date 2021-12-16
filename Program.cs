using System;
using System.IO;
using System.Diagnostics;
using ConcDecoder;

namespace Decoder
{
    // The values within WorkingParams can change during the experiments. 
    class WorkingParams
    {
        public const string studentNumberOne = ""; // This must be filled.
        public const string studentNumberTwo = ""; // This must be filled. Keep it "" if you are working alone.
        public const string classNumber = "INF2A"; // This must be filled. INF2A is just an example.

        public const int numOfWorkers = 40; // how many workers are needed to keep the max size of the shared buffer between 50 - 100?
        public const int minSendIntervalTime = 50; // min sending interval time (in msec) by the provider
        public const int maxSendIntervalTime = 500; // max sending interval time (in msec) by the provider
    }

    // The values of FixedParams must not change in the final submission.
    class FixedParams
    {
        public const int maxNumOfChallenges = 500; // max number of challenges to be solved from the input file
        public const int minTaskExeTime = 1000;  // min execution time (in msec) for abstract tasks
        public const int maxTaskExeTime = 3000;  // max execution time (in msec) for abstract tasks
        public const int terminatingTaskId = -1;  // the id of the terminating task: to terminate the worker(s)
        public const int numOfProviders = 1; // number of the task providers
        public const int rate = 2; // a rate to capture snapshot of the buffer size
        public const string buffVisSymbol = "*"; // a visualisation symbol to depict the growth of the buffer
        public const char delim = ',';
        public const string pad = "8457619249135781";
        public const string inputFileName = @"/challenges.txt";
        public const string logFileName = @"/log.txt";

    }


    class Program
    {
        static void Main(string[] args)
        {
            string logFilePath = "", logFooter = "", logContent = "", logTiming = "" , logResult = "";

            Stopwatch seqSW = new Stopwatch();
            Stopwatch conSW = new Stopwatch();

            seqSW.Start();
            new Launch().SequentialTaskExecution();
            seqSW.Stop();

            TimeSpan seqET = seqSW.Elapsed;

            conSW.Start();
            ConcLaunch concLaunch = new ConcLaunch();
            logResult = concLaunch.ConcurrentTaskExecution(FixedParams.numOfProviders, WorkingParams.numOfWorkers);
            conSW.Stop();

            TimeSpan conET = conSW.Elapsed;

            logTiming =
                "Time Sequential = " + seqET.Minutes + " min, " + seqET.Seconds + "sec, " + seqET.Milliseconds + " msec. " + "\n" +
                "Time Concurrent = " + conET.Minutes + " min, " + conET.Seconds + "sec, " + conET.Milliseconds + " msec. " + "\n";

            logFooter =
                "Number of Loaded Challenges: " + concLaunch.GetLoadedChallenges().ToString() + "\n" +
                "Number of Worker Threads: " + WorkingParams.numOfWorkers + "\n" +
                "Provider min: " + WorkingParams.minSendIntervalTime + "\n" +
                "Provider max: " + WorkingParams.maxSendIntervalTime + "\n" +
                "Class: " + WorkingParams.classNumber + "\n" +
                "Student Number One: " + WorkingParams.studentNumberOne + "\n" +
                "Student Number Two: " + WorkingParams.studentNumberTwo + "\n";

            try
            {
                var basePath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                logFilePath = basePath.Parent.Parent.Parent.FullName;
                logContent = logResult + logTiming + logFooter;
                System.IO.File.WriteAllText(logFilePath + FixedParams.logFileName, logContent);
            }
            catch (Exception e){ Console.WriteLine(e.ToString()); }

            Console.WriteLine(logContent);
        }
    }
}
