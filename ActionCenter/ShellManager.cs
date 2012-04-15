using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using NX.Collections;
using NX.Log.NeuroLog;

namespace NX_Overmind.Actions
{
    public class ShellManager
    {
        private OvermindLogManager _neuroLog = new OvermindLogManager("NeuroLog", OvermindLogManager.LogEntryType.ActionCenter_Shell);

        /// <summary>
        /// Dispatch data over the network
        /// </summary>
        public event ActionCenter.ActionCenterInternalEventHandler DispatchData = null;

        public delegate void ShellManagerEventHandler(int owner, string msg);
        public event ShellManagerEventHandler OutputReceived = null;

        private Queue<byte[]> _cmdQueue = null;
        private Process _shellProc = null;
        private readonly object _processLock = new object();
        private AsyncWorkerQueue _asyncQueue = null;
        private int _owner = 0;

        private Thread _outputReadThread;
        private Thread _errorReadThread;
        private string _bufferOut = "";
        private int _pollingInterval = 200;

        public ShellManager(ActionCenter.ActionCenterInternalEventHandler dispatchData)
        {
            this.DispatchData = dispatchData;
            this._cmdQueue = new Queue<byte[]>();
            this._asyncQueue = new AsyncWorkerQueue();
            this._neuroLog.Write("Initializing Shell Manager");
            //this.LaunchShell();            
        }

        public void EncodeEvent(int id, string str)
        {
            this._neuroLog.WriteFormat("Encode Event", "Id: {0}\nMessage: {1}", id, str);
            this.DispatchData(id, ActionCenter.ActionType.ShellInput, UnicodeEncoding.UTF8.GetBytes(str));
        }

        public void DecodeEvent(int owner, byte[] data, bool input)
        {
            this._neuroLog.WriteFormat("Decode Event", "Owner: {0}\nIs Input: {1}\n\nData:\n{2}", owner, input, data);
            if (input)
            {
                lock (this._cmdQueue)
                {
                    this._owner = owner;
                    this._cmdQueue.Enqueue(data);
                    this._asyncQueue.QueueItem(new AsyncWorkerQueue.AsyncItemMethod(ExecuteCommand));
                }
            }
            else
            {
                if (this.OutputReceived != null)
                {
                    string st = UnicodeEncoding.UTF8.GetString(data);
                    this.OutputReceived(owner, st);
                    this._neuroLog.WriteFormat("Output Received", "String: {0}", st);
                }
            }
        }

        private void ExecuteCommand()
        {
            lock (this._cmdQueue)
            {
                if (this._cmdQueue.Count != 0)
                {
                    lock (this._processLock)
                    {
                        if (this._shellProc == null)                        
                            this.LaunchShell();
                        
                        string input = UnicodeEncoding.UTF8.GetString(this._cmdQueue.Dequeue());
                        input = input.TrimStart(new char[] { '\v' });
                        this._neuroLog.WriteFormat("Execute Command", "Input: {0}", input);
                        this._shellProc.StandardInput.WriteLine(input);
                    }
                }
            }
        }
        
        private void PollForDispatch()
        {
            while (true)
            {
                string st = "";
                lock (this._bufferOut)
                {
                    st = this._bufferOut;
                    this._bufferOut = "";
                }
                if (st.Length > 0)
                {
                    this._neuroLog.WriteFormat("Polled Dispatch", "Message: {0}\n\nData:\n{1}", st, UnicodeEncoding.UTF8.GetBytes(st));
                    this.DispatchData(this._owner, ActionCenter.ActionType.ShellOutput, UnicodeEncoding.UTF8.GetBytes(st));
                }
                Thread.Sleep(_pollingInterval);
            }
        }

        private void ReadOutput()
        {
            int rd = 0;
            while ((rd = this._shellProc.StandardOutput.Read()) > 0)
            {
                if (rd != '\r' && rd != '\v' && rd != '\f')
                    lock (this._bufferOut)
                        this._bufferOut += ((char)rd).ToString();                
            }
        }

        private void ReadError()
        {
            int rd = 0;
            while ((rd = this._shellProc.StandardError.Read()) > 0)
            {
                // DOES NOT SUPPORT ERROR STREAM AS OF NOW

                /*while (this._outputThreadAge != 0)
                    Thread.Sleep(200);
                this._errorThreadAge++;*/
                /*
                 if (rd != '\r')
                    lock (this._bufferOut)
                        this._bufferOut += ((char)rd).ToString(); 
                 */
            }
        }

        private void LaunchShell()
        {
            ProcessStartInfo psInfo = new ProcessStartInfo();
            psInfo.FileName = "cmd.exe";
            psInfo.Verb = "Open";
            psInfo.WorkingDirectory = "";

            psInfo.UseShellExecute = false;
            psInfo.WindowStyle = ProcessWindowStyle.Normal;
            psInfo.CreateNoWindow = true;

            psInfo.RedirectStandardOutput = true;
            psInfo.RedirectStandardInput = true;
            psInfo.RedirectStandardError = true;


            this._shellProc = new Process();
            this._shellProc.EnableRaisingEvents = true;
            this._shellProc.StartInfo = psInfo;
            //this._shellProc.OutputDataReceived += new DataReceivedEventHandler(ps_OutputDataReceived);
            //this._shellProc.ErrorDataReceived += new DataReceivedEventHandler(ps_ErrorDataReceived);
            this._shellProc.Exited += new EventHandler(ps_Exited);

            this._shellProc.Start();
            this._shellProc.StandardInput.AutoFlush = true;

            this._outputReadThread = (new Thread(this.ReadOutput));
            this._outputReadThread.Start();
            this._errorReadThread = (new Thread(this.ReadError));
            this._errorReadThread.Start();
            (new Thread(this.PollForDispatch)).Start();
            this._neuroLog.Write("Launched Shell");
        }

        void ps_Exited(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Quitting Shell");
            this._neuroLog.Write("Exiting Shell");
            lock (this._processLock)
            {
                this._shellProc = null;
            }
        }
    }
}
