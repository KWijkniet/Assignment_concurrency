using System;
using System.Threading;
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
        public readonly Object mutex;

        public ConcurrentTaskBuffer() : base()
        {
            //todo: implement this method such that satisfies a thread safe shared buffer.
            mutex = new object();
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
            lock (this.mutex)
            {
                if (this.taskBuffer.Count > 0)
                {
                    t = this.taskBuffer.Dequeue();
                    if (t.id < 0)
                        this.taskBuffer.Enqueue(t);
                }
            }

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

            //todo: implement this method such that satisfies a thread safe shared buffer.
            Provider provider = new Provider(tasks, this.challenges);
            provider.SendTasks();

            Thread[] threads = new Thread[numOfWorkers];
            Worker[] workers = new Worker[numOfWorkers];
            for (int i = 0; i < numOfWorkers; i++)
            {
                Worker worker = new Worker(tasks);
                threads[i] = new Thread(new ThreadStart(() => {
                    worker.ExecuteTasks();
                }));
                threads[i].Start();
                workers[i] = worker;
            }

            for (int i = 0; i < numOfWorkers; i++)
            {
                threads[i].Join();
            }

            return tasks.GetLogs();
        }
    }
}
