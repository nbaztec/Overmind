using System;
using System.Collections.Generic;
using System.Text;

namespace NX_Overmind
{
    class LogQueue<T>
    {
        private Queue<T> _log = null;

        public LogQueue()
        {
            this._log = new Queue<T>();
        }        

        public void Enqueue(T entry)
        {
            lock (this)
            {
                this._log.Enqueue(entry);
            }
        }

        public T Dequeue()
        {
            lock (this)
            {
                return this._log.Dequeue();
            }
        }

        public void Clear()
        {
            lock (this)
            {
                this._log.Clear();
            }
        }

        public T[] ToArray()
        {
            lock (this)
            {
                return this._log.ToArray();
            }
        }
        
        public T[] Flush()
        {
            lock (this)
            {
                T[] items = this._log.ToArray();
                this._log.Clear();
                return items;
            }
        }
    }
}
