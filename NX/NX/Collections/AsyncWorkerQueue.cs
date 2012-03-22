using System;
using System.Collections.Generic;
using System.Threading;

namespace NX.Collections
{       
    /// <summary>
    /// Queues up methods which will be executed asynchronously but in a serial order
    /// </summary>
    public class AsyncWorkerQueue
    {
        /// <summary>
        /// Delegate to the method to be called
        /// </summary>
        public delegate void AsyncItemMethod();

        protected Queue<AsyncItemMethod> __bufferQueue = null;
        protected volatile bool __newItem;
        protected readonly object __serialLock = new object();      // Ensures that the methods are executed serially
        protected readonly object __itemTestLock = new object();    // Used for checking the queue

        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncWorkerQueue()
        {
            this.__newItem = false;
            this.__bufferQueue = new Queue<AsyncItemMethod>();            
        }

        /// <summary>
        /// Enqueue a method
        /// </summary>
        /// <param name="method">Delegate to the method</param>
        public void QueueItem(AsyncItemMethod method)
        {
            lock (this.__bufferQueue)
            {
                this.__bufferQueue.Enqueue(method);                
            }
            lock (this.__itemTestLock)      
            {
                if (!this.__newItem)        // Check if the worker thread is running
                {
                    this.__newItem = true;
                    Thread t = new Thread(new ThreadStart(this.ProcessQueue));
                    t.Name = "ProcessQueue";
                    t.Start();
                }
            }
        }

        /// <summary>
        /// Processes the queue
        /// </summary>
        protected void ProcessQueue()
        {
            bool cont = false;
            
            while(true)
            {
                lock (this.__itemTestLock)
                {
                    cont = this.__newItem;
                    this.__newItem = false;
                }
                if (!cont)                                              // If there is no new item, then exit
                    break;

                AsyncItemMethod[] methods = null;
                lock (this.__bufferQueue)                               // Get all methods
                {                    
                    if (this.__bufferQueue.Count > 0)
                    {
                        methods = this.__bufferQueue.ToArray();
                        this.__bufferQueue.Clear();
                    }
                }

                lock (this.__serialLock)
                {
                    if (methods != null)
                    {
                        foreach (AsyncItemMethod method in methods) //Execute all methods
                            method();
                    }
                }
            }
        }
    }
}
