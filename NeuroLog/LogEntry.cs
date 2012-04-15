using System;
using System.IO;
using System.Text;

namespace NX.Log.NeuroLog
{
    public class LogEntry
    {
        private DateTime _timestamp;
        private short _tag;
        private string _message;
        private string _details;

        public DateTime Timestamp { get { return this._timestamp; } }
        public short EntryType { get { return this._tag; } }
        public string Message { get { return this._message; } }
        public string Details { get { return this._details; } }

        public LogEntry()
        {
        }

        public LogEntry(byte[] data)
            : this()
        {
            this.Deserialize(data);
        }

        public LogEntry(short tag, string message, string details)
            : this()
        {
            this._timestamp = DateTime.Now;
            this._tag = tag;
            this._message = message;
            this._details = details;
        }

        public LogEntry(short tag, string message)
            : this(tag, message, "")
        {        
        }

        public byte[] Serialize()
        {
            byte[] _t;
            using (MemoryStream ms = new MemoryStream())
            {
                _t = BitConverter.GetBytes(this._timestamp.Ticks);
                ms.Write(_t, 0, _t.Length);
                _t = BitConverter.GetBytes(this._tag);
                ms.Write(_t, 0, _t.Length);

                _t = BitConverter.GetBytes(this._message.Length);
                ms.Write(_t, 0, _t.Length);
                _t = UnicodeEncoding.UTF8.GetBytes(this._message);
                ms.Write(_t, 0, _t.Length);

                _t = BitConverter.GetBytes(this._details.Length);
                ms.Write(_t, 0, _t.Length);
                _t = UnicodeEncoding.UTF8.GetBytes(this._details);
                ms.Write(_t, 0, _t.Length);

                _t = ms.ToArray();
            }
            return _t;
        }

        public int Deserialize(byte[] data)
        {
            int r = 0;
            using (MemoryStream ms = new MemoryStream(data))
            {
                byte[] _t = new byte[sizeof(long)];
                r += ms.Read(_t, 0, _t.Length);
                long time = BitConverter.ToInt64(_t, 0);
                this._timestamp = new DateTime(time, DateTimeKind.Utc);

                _t = new byte[sizeof(short)];
                r += ms.Read(_t, 0, _t.Length);
                this._tag = BitConverter.ToInt16(_t, 0);

                _t = new byte[sizeof(int)];
                r += ms.Read(_t, 0, _t.Length);
                int str_len = BitConverter.ToInt32(_t, 0);
                _t = new byte[str_len];                
                r += ms.Read(_t, 0, _t.Length);
                this._message = UnicodeEncoding.UTF8.GetString(_t);

                _t = new byte[sizeof(int)];
                r += ms.Read(_t, 0, _t.Length);
                str_len = BitConverter.ToInt32(_t, 0);
                _t = new byte[str_len];                
                r += ms.Read(_t, 0, _t.Length);
                this._details = UnicodeEncoding.UTF8.GetString(_t);

                _t = ms.ToArray();
            }
            return r;
        }
    }

}
