using System;
using Decoder;

namespace ConcDecoder
{
    /// <summary>
    /// A concurrent version of the class Buffer
    /// Note: For the final solution this class MUST be implemented.
    /// </summary>
    public class ConcurrentTaskBuffer : TaskBuffer
    {
        //todo: add required fields such that satisfies a thread safe shared buffer.

        public ConcurrentTaskBuffer() : base()
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
        }

        /// <summary>
        /// Adds the given task to the queue. The implementation must support concurrent accesses.
        /// </summary>
        /// <param name="task">A task to wait in the queue for the execution</param>
        public override void AddTask(TaskDecryption task)
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            Console.WriteLine("add task programm has been called");
            this.taskBuffer.Enqueue(task);// gooi task in queue
            this.numOfTasks++;// tell hoeveel task
            this.maxBuffSize = this.taskBuffer.Count > this.maxBuffSize ? this.taskBuffer.Count : this.maxBuffSize;//changing buff size depending on wat groter is

            this.LogVisualisation();//idk 
            this.PrintBufferSize();// printen van buffer
        }

        /// <summary>
        /// Picks the next task to be executed. The implementation must support concurrent accesses.
        /// </summary>
        /// <returns>Next task from the list to be executed. Null if there is no task.</returns>
        public override TaskDecryption GetNextTask()
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            TaskDecryption t = null;

            return t;
        }

        /// <summary>
        /// Prints the number of elements available in the buffer.
        /// </summary>
        public override void PrintBufferSize()
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            Console.Write("Buffer#{0} ; ", this.taskBuffer.Count);
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
            Provider provider = new Provider(tasks, this.challenges);
            Worker[] worker = new Worker[numOfWorkers];
            for (int i = 0; i < numOfWorkers; i++)
            {
                worker[i] = new Worker(tasks);
            }


            //todo: implement this method such that satisfies a thread safe shared buffer.


            return tasks.GetLogs();
        }
    }
}
