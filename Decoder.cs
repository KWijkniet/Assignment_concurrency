using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace Decoder
{
    /// <summary>
    /// A base class for a task.
    /// Note: This class MUST NOT change.
    /// </summary>
    public class Task
    {
        protected string taskInput;
        protected string taskOutput;
        public int id;

        public Task(int id, string input)
        {
            this.id = id;
            this.taskInput = input;
        }

        public virtual void Execute()
        {
            //Console.Clear();
            Console.Write("Execution:{0} ; ",this.id);
            if(this.id > 0)
            {
                Thread.Sleep(new Random().Next(FixedParams.minTaskExeTime, FixedParams.maxTaskExeTime));
                this.taskOutput = this.taskInput.ToUpper();
            }
        }

        public virtual string GetResult(){ return this.taskOutput; }
    }

    /// <summary>
    /// A task class for decryption.
    /// NOTE: This class MUST NOT change.
    /// </summary>
    public class TaskDecryption : Task
    {
        private string hash;
        private long challenge;
        private int maxPrimeRange = 15000;
        private LinkedList<int> primes;
        private Tuple<int, int> result;

        public TaskDecryption(int id, string input) : base(id,input)
        {
            this.primes = new LinkedList<int>();
            if(id > FixedParams.terminatingTaskId)
            {
                string[] words = input.Split(FixedParams.delim);
                if (words is not null && words.Length > 0)
                {
                    this.challenge = Convert.ToInt64(words[0]);
                    this.hash = words[1];
                }
            }
        }
        /// <summary>
        /// Given the challenges, this method finds a solution.
        /// </summary>
        public override void Execute()
        {
            string hashStr;
            this.result = new Tuple<int, int>(-1, -1);
            this.CollectPrimes(1, this.maxPrimeRange);
            int[] primesArray = new int[primes.Count];
            primes.CopyTo(primesArray, 0);

            for (int j = 0; j < this.primes.Count; j++)
                for (int i = 0; i < this.primes.Count; i++)
                {
                    hashStr = GetHash(this.challenge, primesArray[i], primesArray[j]);
                    if (hashStr == this.hash)
                        this.result = new Tuple<int, int>(primesArray[i], primesArray[j]);
                }
            Console.WriteLine("TaskID:{0}[(Challenge:{1},Hash:{2}) ; Keys:{3}]",this.id, this.challenge , this.hash , this.GetResult());
        }

        /// <summary>
        /// Given number and two prime numbers returns the hash as a string
        /// </summary>
        private string GetHash(long num, int key1, int key2)
        {
            long x, y, z;

            try
            {
                x = num % key1;
            }catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            string xstr = x.ToString();
            string xpaded = xstr + FixedParams.pad.Substring(0, 8 - xstr.Length);
            y = long.Parse(xpaded);
            try
            {
                double re = Math.Pow(y, 1.0 / 3);
                z = (long)(Math.Pow(y, 1.0 / 3) * key2);
            }catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            string zstr = z.ToString();
            return zstr+FixedParams.pad.Substring(0, 8 - zstr.Length);
        }

        /// <summary>
        /// This method collects all the prime numbers between lower and upper.
        /// </summary>
        /// <returns> -1 if lower > upper, 0 otherwise which indicates the prime numbers successfully.</returns>
        private int CollectPrimes(int lower, int upper)
        {
            bool isPrime = true;

            if (lower > upper)
                return -1; //

            for (int n = lower; n <= upper; n++)
            {
                isPrime = true; // assume n is a prime number
                for (int i = 2; i < n && isPrime; i++)
                    if (n % i == 0)
                        isPrime = false; // our assumption was not correct
                if (isPrime)
                    this.primes.AddLast(new LinkedListNode<int>(n)); // add prime number if our assumption was correct
            }
            return 0;
        }

        public override string GetResult()
        {
            this.taskOutput = this.result.ToString();
            return base.GetResult();
        }
    }
    /// <summary>
    /// A task buffer: a wrapper for a queue of tasks.
    /// Note: This class MUST NOT change.
    /// </summary>
    public class TaskBuffer
    {
        protected Queue<TaskDecryption> taskBuffer;
        protected string buffSizeLog;
        protected int logCounter;
        protected int numOfTasks;
        protected int maxBuffSize;

        public TaskBuffer()
        {
            this.logCounter = 0;
            this.numOfTasks = 0;
            this.maxBuffSize = 0;
            this.buffSizeLog = "";
            this.taskBuffer = new Queue<TaskDecryption>();
        }

        /// <summary>
        /// Adds the given task to the queue.
        /// </summary>
        /// <param name="task">A task to wait in the queue for the execution</param>
        public virtual void AddTask(TaskDecryption task)
        {
            this.taskBuffer.Enqueue(task);
            this.numOfTasks++;
            this.maxBuffSize = this.taskBuffer.Count > this.maxBuffSize ? this.taskBuffer.Count  : this.maxBuffSize;

            this.LogVisualisation();
            this.PrintBufferSize();
        }

        /// <summary>
        /// Picks the next task to be executed. 
        /// </summary>
        /// <returns>Next task from the list to be executed. Null if there is no task.</returns>
        public virtual TaskDecryption GetNextTask()
        {
            TaskDecryption t = null;
            if (this.taskBuffer.Count > 0)
            {
                t = this.taskBuffer.Dequeue();
                // check if the task is the last ending task: put the task back.
                // It is an indication to terminate processors
                if (t.id < 0)
                    this.taskBuffer.Enqueue(t);
            }
            return t;
        }

        /// <summary>
        /// Prints the number of elements available in the buffer.
        /// </summary>
        public virtual void PrintBufferSize()
        {
            //Console.Clear();
            Console.Write("Buffer#{0} ; ", this.taskBuffer.Count);
        }

        public string GetLogs()
        {
            string log = "The growth of the buffer:\n" + this.buffSizeLog;
            log = log + "\n" + "Number of Tasks: " + this.numOfTasks + "\n" + "Max Buffer Size: " + this.maxBuffSize + "\n";
            return log;
        }

        /// <summary>
        /// Keeps track of the growth of the buffer.
        /// It does not provide a precise growth, but just an symbolic indication.
        /// </summary>
        public void LogVisualisation()
        {
            int indic = this.taskBuffer.Count/FixedParams.rate;
            this.logCounter++;
            if((this.logCounter % FixedParams.rate) == 0)
            {
                for (int c = 0; c < indic; c++)
                    buffSizeLog = buffSizeLog + FixedParams.buffVisSymbol;
                buffSizeLog = buffSizeLog + "\n";
            }
        }

    }

    /// <summary>
    /// The Provider class implements functionalities of feedings tasks to the shared buffer.
    /// This class MUST NOT change.
    /// </summary>
    public class Provider
    {
        private TaskBuffer tasksBuffer;
        private string[] challenges;

        public Provider(TaskBuffer tasks, string[] challenges)
        {
            this.tasksBuffer = tasks;
            this.challenges = challenges;
        }
        /// <summary>
        /// Periodically (every t miliseconds) adds the next task to the buffer.
        /// t is a random value between WorkingParams.minSendIntervalTime and WorkingParams.maxSendIntervalTime.
        /// Higher values of t means slower provider and lower values for t means faster provider.
        /// </summary>
        public void SendTasks()
        {
            // after a random delay, add the task to the shared buffer
            for (int indx = 0; indx < challenges.Length; indx++)
            {
                this.tasksBuffer.AddTask(new TaskDecryption(indx, challenges[indx]));
                Thread.Sleep(new Random().Next(WorkingParams.minSendIntervalTime, WorkingParams.maxSendIntervalTime));
            }
            // the last task with id < 0 is an indication for the terminating task
            this.tasksBuffer.AddTask(new TaskDecryption(FixedParams.terminatingTaskId, ""));
        }
    }

    /// <summary>
    /// The Worker class implements functionalities of executing tasks provided in a given buffer.
    /// Note: This class MUST NOT change.
    /// </summary>
    public class Worker
    {
        private TaskBuffer tasksBuffer;
        private bool terminate;

        /// <summary>
        /// Inistantiates an object to execute given tasks.
        /// </summary>
        /// <param name="tasks"> A queue of tasks to be executed by the worker.</param>
        public Worker(TaskBuffer tasks)
        {
            this.tasksBuffer = tasks;
            this.terminate = false;
        }

        /// <summary>
        /// Picks the next task from the buffer and executes the task, if it is not the "terminating" task.
        /// Terminating task: a task with an id < 0
        /// </summary>
        public void ExecuteNext()
        {
            Task t = this.tasksBuffer.GetNextTask();
            if (t is not null)
                if (t.id == FixedParams.terminatingTaskId)
                    this.terminate = true;
                else
                    t.Execute();
        }

        /// <summary>
        /// Execute the tasks as long as the terminating task is not seen.
        /// </summary>
        public void ExecuteTasks()
        {
            while (!terminate)
                this.ExecuteNext();
        }
    }

    /// <summary>
    /// Launch prepares the environment and objects to execute the whole experiment.
    /// </summary>
    class Launch
    {
        protected string[] challenges;

        public Launch()
        {
            try
            {
                this.challenges = new string[FixedParams.maxNumOfChallenges];
                var basePath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                string inputFilePath = basePath.Parent.Parent.Parent.FullName;
                string[] inputContent = System.IO.File.ReadAllLines(inputFilePath + FixedParams.inputFileName);
                Array.Copy(inputContent, this.challenges, FixedParams.maxNumOfChallenges);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        public void SequentialTaskExecution()
        {
            TaskBuffer tasks = new TaskBuffer();
            Provider provider = new Provider(tasks, this.challenges);
            Worker worker = new Worker(tasks);

            provider.SendTasks();
            worker.ExecuteTasks();
        }

        public int GetLoadedChallenges() { return this.challenges.Length; }

    }
}
