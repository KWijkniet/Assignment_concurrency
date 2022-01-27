using System;
using Decoder;
using System.Collections.Generic;




namespace ConcDecoder
{
    /// <summary>
    /// A concurrent version of the class Buffer
    /// Note: For the final solution this class MUST be implemented.
    /// </summary>
    public class ConcurrentTaskBuffer : TaskBuffer
    {
        //todo: add required fields such that satisfies a thread safe shared buffer.
        protected Queue<TaskDecryption> taskBuffer;
        protected string buffSizeLog;
        protected int logCounter;
        protected int numOfTasks;
        protected int maxBuffSize;

        public ConcurrentTaskBuffer() : base()
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            this.logCounter = 0;
            this.numOfTasks = 0;
            this.maxBuffSize = 0;
            this.buffSizeLog = "";
            this.taskBuffer = new Queue<TaskDecryption>();
        }

        /// <summary>
        /// Adds the given task to the queue. The implementation must support concurrent accesses.
        /// </summary>
        /// <param name="task">A task to wait in the queue for the execution</param>
        public override void AddTask(TaskDecryption task)
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            this.taskBuffer.Enqueue(task);
            this.numOfTasks++;
            this.maxBuffSize = this.taskBuffer.Count > this.maxBuffSize ? this.taskBuffer.Count : this.maxBuffSize;

            this.LogVisualisation();
            this.PrintBufferSize();
        }

        /// <summary>
        /// Picks the next task to be executed. The implementation must support concurrent accesses.
        /// </summary>
        /// <returns>Next task from the list to be executed. Null if there is no task.</returns>
        public override TaskDecryption GetNextTask()
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            TaskDecryption t = null;
            if (this.taskBuffer.Count > 0)
            {
                t = this.taskBuffer.Dequeue();
                if (t.id < 0)
                    this.taskBuffer.Enqueue(t);
            }

            return t;
        }

        /// <summary>
        /// Prints the number of elements available in the buffer.
        /// </summary>
        public override void PrintBufferSize()
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            Console.WriteLine("Buffer#{0} ; ", this.taskBuffer.Count);
        }
    }

    class ConcLaunch : Launch
    {
        public ConcLaunch() : base(){  }

        /// <summary>
        /// This method implements the concurrent version of the decryption of provided challenges.
        /// </summary>
        /// <param name="numOfProviders">Number of providers</param>
        /// <param name="numOfWorkers">Number of workers</param>
        /// <returns>Information logged during the execution.</returns>
        public string ConcurrentTaskExecution(int numOfProviders, int numOfWorkers)
        {
            ConcurrentTaskBuffer tasks = new ConcurrentTaskBuffer();

            //todo: implement this method such that satisfies a thread safe shared buffer.
            Provider provider = new Provider(tasks, challenges);
            Worker[] workers = new Worker[numOfWorkers];
            for (int i = 0; i < numOfWorkers; i++)
            {
                workers[i] = new Worker(tasks);
            }
            


            provider.SendTasks();
            



            return tasks.GetLogs();
        }
    }
}
