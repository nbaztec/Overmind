using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NX.Log.NeuroLog
{    
    public class OvermindLogManager
    {
        private static readonly object writeLock = new object();

        public enum LogEntryType : short
        {
            None = 0,
            App_Launcher,
            App_Server,
            App_Client,
            NX_Net,
            ActionCenter,
            ActionCenter_Capture,
            ActionCenter_Shell,
            ActionCenter_Event
        }

        private string _logFile = null;
        private LogEntryType _tag = 0;
        public string Filename { get { return this._logFile; } }

        public OvermindLogManager(string filename, LogEntryType tag)
        {
            this._logFile = filename;
            this._tag = tag;
        }

        public OvermindLogManager(string filename)
            : this(filename, LogEntryType.None)
        {            
        }

        public void Erase()
        {
            lock (writeLock)
            {
                if (File.Exists(this._logFile))
                    File.Delete(this._logFile);
            }
        }

        public void Write(LogEntryType type, string message, string details)
        {
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {            
                this.WriteToFileSync(type, message, details);
            })).Start();
        }
        
        private void WriteToFileSync(LogEntryType type, string message, string details)
        {
            lock (writeLock)
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream(this._logFile, FileMode.Append, FileAccess.Write)))
                {
                    byte[] data = (new LogEntry((short)type, message, details).Serialize());
                    bw.Write(data.Length);
                    bw.Write(data);
                }
            }
        }

        public void Write(LogEntryType type, string message)
        {
            this.Write(type, message, "");
        }

        public void Write(string message, string details)
        {
            this.Write(this._tag, message, details);
        }

        public void WriteFormat(string message, string format, params object[] args)
        {            
            new System.Threading.Thread(new System.Threading.ThreadStart(delegate
            {
                for (int i = 0; i < args.Length; i++)
                    if (args[i] is byte[])
                        args[i] = args[i] == null ? "[null]" : (args[i] as byte[]).Length.ToString();    // this.GetHexString(args[i] as byte[]);
                                                                                        // Disabled for performance issues. 

                this.WriteToFileSync(this._tag, message, String.Format(format, args));
            })).Start();
        }

        public void Write(string message)
        {
            this.Write(this._tag, message, "");
        }

        private long _readPointer = 0;
        public LogEntry ReadNext()
        {
            LogEntry log = null;
            int offset = 0;            
            using (BinaryReader br = new BinaryReader(new FileStream(this._logFile, FileMode.Open, FileAccess.Read)))
            {
                if (this._readPointer < br.BaseStream.Length)
                {
                    br.BaseStream.Position = this._readPointer;
                    int l = br.ReadInt32();
                    byte[] data = new byte[l];
                    br.Read(data, 0, data.Length);
                    log = new LogEntry(data);
                    offset = sizeof(int) + data.Length;
                }
            }
            this._readPointer += offset;
            return log;
        }

        public void SeekToFirst()
        {
            this._readPointer = 0;
        }

        public void SeekTo(long position)
        {
            this._readPointer = position;
        }

        public string GetHexString(byte[] data)
        {
            if (data == null)
                return "[null]";
            else
            {
                string t = "";
                for (int i = 0; i < data.Length; i++)
                {
                    t += string.Format("{0:X2} ", data[i]);
                    if ((i+1) % 16 == 0)
                        t += "\n";
                }
                return t.Trim();
            }
            //return "";
        }
    }
}
