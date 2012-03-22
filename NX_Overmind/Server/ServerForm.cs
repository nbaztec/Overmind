using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using NX.Net;
using NX.Controls;
using NX.Collections;

using NX_Overmind.Actions;

namespace NX_Overmind
{
    public partial class ServerForm : Form
    {
        private class TabImagePair
        {
            public readonly Image Screen;
            public readonly int Tab;

            public TabImagePair(int tab, Image screen)
            {
                this.Tab = tab;
                this.Screen = screen;
            }
        }
        
        /**
         * Params
         * 
         */
        private int _paramSocket;

        private TcpServer _server = null;
        private Dictionary<int, ActionCenter> _actionCenters = null;
        //private ActionCenter _actionCenter = null;

        private AsyncWorkerQueue _workerQueue = null;
        private Dictionary<int, OvermindClientTab> _tabClientPageIndices = null;
        private Queue<TabImagePair> _snapQueue = null;
        private string _parentRecordDir = null;
        private Dictionary<int, CaptureRecorder> _captureRecorders = null;

        public ServerForm()
        {
            InitializeComponent();            
        }

        public ServerForm(int socket)
            :this()
        {
            this._paramSocket = socket;
            this._parentRecordDir = System.IO.Path.Combine(Application.StartupPath, "Recordings");
            if(!System.IO.Directory.Exists(this._parentRecordDir))
                System.IO.Directory.CreateDirectory(this._parentRecordDir);
            
            this.tabControl.TabPages.Add(new OvermindClientTab(0, "NB", null));
            this._captureRecorders = new Dictionary<int, CaptureRecorder>();
            this._tabClientPageIndices = new Dictionary<int, OvermindClientTab>();
            this._workerQueue = new AsyncWorkerQueue();
            this._snapQueue = new Queue<TabImagePair>();
            // Quantizer _quantizer = new OctreeQuantizer(256, 8);

            this._actionCenters = new Dictionary<int, ActionCenter>();
            
            this._server = new TcpServer();

            this._server.OnServerStart += new TcpServer.ServerStatusEventHandler(_server_OnServerStart);
            this._server.OnServerStop += new TcpServer.ServerStatusEventHandler(_server_OnServerStop);
            this._server.OnError += new TcpServer.ServerStatusEventHandler(_server_OnError);
            this._server.OnClientConnected += new TcpServer.ServerStatusEventHandler(_server_OnClientConnected);
            this._server.OnDataReceived += new TcpServer.ServerStatusEventHandler(_server_OnDataReceived);
            this._server.OnAcknowledged += new TcpServer.ServerStatusEventHandler(_server_OnAcknowledged);
            this._server.OnStatusChanged += new TcpServer.ServerStatusEventHandler(_server_OnStatusChanged);
            this._server.OnClientStatusChanged += new TcpServer.ServerClientStatusEventHandler(_server_OnClientStatusChanged);

            this._server.Start(this._paramSocket);

            
            /*
            Bitmap bmp = ScreenSnap.ScreenSnapshot();
            //Bitmap bmp = (Bitmap)Image.FromFile("test.bmp", false);
            long l1 = DateTime.UtcNow.Ticks;
            ScreenSnap.FI_ConvertSave(bmp, 0.4f);
            System.Diagnostics.Trace.WriteLine("[FI]* : " + (new TimeSpan(DateTime.UtcNow.Ticks-l1)).TotalSeconds);
            NX.Imaging.OctreeQuantizer oq = new NX.Imaging.OctreeQuantizer(256, 8);
            //NX.Imaging.UniformQuantizer oq = new NX.Imaging.UniformQuantizer();            
            l1 = DateTime.UtcNow.Ticks;
            ScreenSnap.SnapshotToFile(oq.Quantize(ScreenSnap.ShrinkBitmap(bmp, 0.4f)), "test_z3.png");
            System.Diagnostics.Trace.WriteLine("[OT]# : " + (new TimeSpan(DateTime.UtcNow.Ticks-l1)).TotalSeconds);
            
            //ScreenSnap.SnapshotToFile(bmp, "test_z2.png", 75, 0.4f);      */      
            // END
            //this._server.WaitForClient();
        }

        /*private OvermindClientTab GetClientTab(NetPacket packet)
        {
            //return (OvermindClientTab)this.tabControl.TabPages[this._server.GetReceiverName(packet)];
            OvermindClientTab tab = (OvermindClientTab)this.tabControl.TabPages["#" + packet.Header.Source.ToString()];
            return tab;
        }*/
        
        private void UpdateTerminal(RichTextConsole console, Color c, object obj)
        {
            try
            {                             
                if (obj is NetEventArgs)
                {
                    NetEventArgs e = obj as NetEventArgs;
                    if (e.Message != null)                    
                        console.TerminalAppendLine(e.Message, c);                                            
                }
                else if (obj is String)                
                    console.TerminalAppendLine(obj as string, c);               
            }
            catch (Exception) { }
        }

        void _server_OnStatusChanged(object sender, NetEventArgs e)
        {
            //this.tabControl.TabPages[this._server.GetReceiverName(e.Packet)].Text = "A";
            this.UpdateTerminal(this.textLog, Color.YellowGreen, "[S]: Status Changed, " + e.Message);
        }

        void _server_OnAcknowledged(object sender, NetEventArgs e)
        {            
            this.Invoke((MethodInvoker)delegate {
                ActionCenter ac = new ActionCenter("l*l7k0d3");
                ac.SendData += new ActionCenter.ActionCenterEventHandler(this._actionCenter_SendData);
                ac.CaptureManagerModule.ReceivedCapture += new ActionCenter.CaptureEventHandler(this._actionCenter_ReceivedCapture);
                ac.CaptureManagerModule.ReceivedCursor += new CaptureManager.CaptureCursorEventHandler(this._actionCenter_ReceivedCursor);
                ac.CaptureManagerModule.DebugEvent += new ActionCenter.DebugEventHandler(this._actionCenter_DebugEvent);                

                ac.ReceiveRawCapture += new ActionCenter.ActionCenterEventHandler(this._actionCenter_ReceiveRawCapture);
                this._actionCenters.Add(e.Packet.Header.Destination, ac);
                OvermindClientTab otab = this.CreateOvermindClientTab(e.Packet.Header.Destination, this._server.GetReceiverName(e.Packet), ac.CaptureManagerModule);

                this._captureRecorders.Add(e.Packet.Header.Destination, new CaptureRecorder(System.IO.Path.Combine(this._parentRecordDir, String.Format("Client {0}\\ID [{0}]", e.Packet.Header.Destination))));

                this._tabClientPageIndices.Add(e.Packet.Header.Destination, otab);
                this.tabControl.TabPages.Add(this._tabClientPageIndices[e.Packet.Header.Destination]);                
            });
            this.UpdateTerminal(this.textLog, Color.Gold, "[" + this._server.GetReceiverName(e.Packet) + "]: Client Acked");
        }
        
        private OvermindClientTab CreateOvermindClientTab(int id, string name, CaptureManager captureModule)
        {
            OvermindClientTab otab = new OvermindClientTab(id, name, captureModule);            
            otab.OnRecordChanged += new OvermindClientTab.ClientTabEventHandler(otab_OnRecordChanged);
            otab.KeyDown += new OvermindClientTab.ClientEventHandler(otab_KeyDown);
            otab.KeyPress += new OvermindClientTab.ClientEventHandler(otab_KeyPress);
            otab.KeyUp += new OvermindClientTab.ClientEventHandler(otab_KeyUp);
            otab.MouseClick += new OvermindClientTab.ClientEventHandler(otab_MouseClick);
            otab.MouseDoubleClick += new OvermindClientTab.ClientEventHandler(otab_MouseDoubleClick);
            otab.MouseDown += new OvermindClientTab.ClientEventHandler(otab_MouseDown);
            otab.MouseMove += new OvermindClientTab.ClientEventHandler(otab_MouseMove);
            otab.MouseUp += new OvermindClientTab.ClientEventHandler(otab_MouseUp);
            otab.MouseWheel += new OvermindClientTab.ClientEventHandler(otab_MouseWheel);

            return otab;
        }

        #region OvermindClientTab Initializers
        void otab_MouseWheel(int id, object sender, EventArgs e)
        {
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.MouseWheel);
        }

        void otab_MouseUp(int id, object sender, EventArgs e)
        {
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.MouseUp);
        }

        void otab_MouseMove(int id, object sender, EventArgs e)
        {
            this.UpdateTerminal(this.textDebug, Color.AntiqueWhite, (e as MouseEventArgs).X.ToString() + ", " + (e as MouseEventArgs).Y.ToString());
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.MouseMove);
        }

        void otab_MouseDown(int id, object sender, EventArgs e)
        {
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.MouseDown);
        }

        void otab_MouseDoubleClick(int id, object sender, EventArgs e)
        {
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.MouseDoubleClick);
        }

        void otab_MouseClick(int id, object sender, EventArgs e)
        {
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.MouseClick);
        }

        void otab_KeyUp(int id, object sender, EventArgs e)
        {
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.KeyUp);
        }

        void otab_KeyPress(int id, object sender, EventArgs e)
        {
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.KeyPress);
        }

        void otab_KeyDown(int id, object sender, EventArgs e)
        {
            this._actionCenters[id].EventManagerModule.EncodeEvent(id, e, NX.Hooks.HookEventCodec.EventType.KeyDown);
        }
        #endregion

        void _actionCenter_SendData(int id, byte[] data)
        {
            if (this._server.Connected)
            {
                this._server.BeginWriteToClient(id, NetPacket.PacketType.Custom, data);
                this.UpdateTerminal(this.textLog, Color.Crimson, String.Format("Sending data to '{0}'", this._tabClientPageIndices[id].Text));
            }
        }

        void otab_OnRecordChanged(object sender, bool enabled)
        {
            //throw new NotImplementedException();
            OvermindClientTab oct = sender as OvermindClientTab;
            this._captureRecorders[oct.ClientId].Enabled = enabled;
            //this._tabClientPageIndices[
            this.UpdateTerminal(this.textLog, Color.Yellow, String.Format("Recording {0} for [{1}]:{2}; File = '{3}'", (enabled? "Enabled": "Disabled"), oct.ClientId, oct.Text, this._captureRecorders[oct.ClientId].LogFilename));
        }

        void _server_OnDataReceived(object sender, NetEventArgs e)
        {
            switch (e.Packet.Header.Type)
            {
                case NetPacket.PacketType.Print:
                    this.UpdateTerminal(this.textLog, Color.Indigo, "[" + this._server.GetSenderName(e.Packet) + "]: Data received, " + UTF8Encoding.UTF8.GetString(e.Packet.Data));
                    break;
                case NetPacket.PacketType.Custom:
                    this.UpdateTerminal(this._tabClientPageIndices[e.Packet.Header.Source].NetworkLog, Color.Indigo, "[" + this._server.GetSenderName(e.Packet) + "]: Data received: Custom");
                    this._actionCenters[e.Packet.Header.Source].InitiateAction(e.Packet.Header.Source, e.Packet.Data);
                    break;
                default:
                    this.UpdateTerminal(this.textLog, Color.Indigo, "[" + this._server.GetSenderName(e.Packet) + "]: Data received");
                    break;
            }
        }

        void _server_OnClientConnected(object sender, NetEventArgs e)
        {            
            this.UpdateTerminal(this.textLog, Color.GreenYellow, "[" + this._server.GetSenderName(e.Packet) + "]: Client Connected");
        }

        void _server_OnError(object sender, NetEventArgs e)
        {
            this.UpdateTerminal(this.textLog, Color.LightPink, "[S]: Error, " + e.Exception.Message);
        }

        void _server_OnServerStop(object sender, NetEventArgs e)
        {
            this.UpdateTerminal(this.textLog, Color.Fuchsia, "[S]: Stop");
        }

        void _server_OnServerStart(object sender, NetEventArgs e)
        {
            this.UpdateTerminal(this.textLog, Color.White, "[S]: Start, " + e.Message);
        }

        void _server_OnClientStatusChanged(int id, string name, TcpCommand cmd)
        {
            switch (cmd.Type)
            {
                case TcpCommand.Command.Disconnect:
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        this.tabControl.TabPages.Remove(this._tabClientPageIndices[id]);
                        this._captureRecorders[id].Dispose();
                    }));
                    break;

                case TcpCommand.Command.Name:
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        this.UpdateTerminal(this.textLog, Color.Yellow, "Changing name to: " + name);
                        this.tabControl.SelectedTab = this._tabClientPageIndices[id];
                        this.tabControl.SelectedTab.Text = name;
                    }));
                    break;
            }
        }      

        private void _actionCenter_DebugEvent(string message)
        {
            this.UpdateTerminal(this.textDebug, Color.OrangeRed, message);
        }

        void _actionCenter_ReceiveRawCapture(int id, byte[] data)
        {
            if (data.Length != 0)            
                this._captureRecorders[id].Log(data);            
        }

        private void _actionCenter_ReceivedCapture(int id, Image screenShot, NX.Hooks.HookEventArgs[] eventArgs)
        {
            string log = HookEventHelper.HookEventsToString(eventArgs);
            if (log != "")
                this.UpdateTerminal(this._tabClientPageIndices[id].CaptureLog, Color.YellowGreen, log);                
            if (screenShot != null)
            {
                this._snapQueue.Enqueue(new TabImagePair(id, screenShot));
                this.StartScreenDisplayTimer();
            }
        }        

        private void _actionCenter_ReceivedCursor(int owner, Cursor cursor, int xHotSpot, int yHotSpot)
        {
            if (cursor != null)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    //System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    //cursor.Save(ms);    
                    //bmp.MakeTransparent(Color.Black);
                    //this.pictureBox1.Image = cursor.ToBitmap();                    
                    this._tabClientPageIndices[owner].ScreenImage.Cursor = cursor;// new Cursor(cursor.ToBitmap().GetHicon()); ;// this.CreateCursor(cursor.ToBitmap(), 0, 0);//new Cursor(ms);
                }));
            }
        }


        private void SendSettings()
        {
            //this._server.BeginWriteToClient(0, NetPacket.CommandType.Custom
        }

        private void StartScreenDisplayTimer()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke((MethodInvoker)delegate() { this.StartScreenDisplayTimer(); });
                }
                else
                {
                    if (!this.snapShow.Enabled)
                        this.snapShow.Enabled = true;
                }
            }
            catch (Exception) { }
        }

        private void snapShow_Tick(object sender, EventArgs e)
        {
            if (this._snapQueue.Count == 0)
                this.snapShow.Enabled = false;
            else
            {
                TabImagePair ti = this._snapQueue.Dequeue();
                this._tabClientPageIndices[ti.Tab].ScreenImage.Image = ti.Screen;                
                //this.UpdateTerminal(this.textDebug, Color.AliceBlue, "Count: " + this._snapQueue.Count);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._server.Stop();
        }
    }
}
