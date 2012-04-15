using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.Sockets;

namespace NX.Net
{
    /// <summary>
    /// Handles the server operations
    /// </summary>
    public class TcpServer
    {
        /// <summary>
        /// Number of clients connected till now (Including the disconnected ones)
        /// </summary>
        private int __clientCounter;   
        /// <summary>
        /// Socket listener
        /// </summary>
        private TcpListener _tcpListener = null;
        /// <summary>
        /// Manages connections with each client
        /// </summary>
        private Dictionary<int, TcpClientManager> _clientManager = new Dictionary<int, TcpClientManager>();
        /// <summary>
        /// Stores the name of each client
        /// </summary>
        private Dictionary<int, string> _clientNames = new Dictionary<int, string>();
        private bool _isRunning = false;
        private String _serverIP;

        public bool Connected { get { return this._isRunning; } }

        /// <summary>
        /// Delegate for invoking server status updates
        /// </summary>        
        public delegate void ServerStatusEventHandler(object sender, NetEventArgs e);
        /// <summary>
        /// Delegate for invoking events related to client
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="name">Client name</param>
        /// <param name="cmd">Command type</param>        
        public delegate void ServerClientStatusEventHandler(int id, string name, TcpCommand cmd);
        
        public event ServerStatusEventHandler OnStatusChanged;
        public event ServerStatusEventHandler OnServerStart;
        public event ServerStatusEventHandler OnServerStop;
        public event ServerStatusEventHandler OnClientConnected;
        public event ServerStatusEventHandler OnDataReceived;
        public event ServerStatusEventHandler OnAcknowledged;        
        public event ServerStatusEventHandler OnError;

        public event ServerClientStatusEventHandler OnClientStatusChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public TcpServer()
        {
            this.__clientCounter = 0;
            this._clientNames.Add(this.__clientCounter, "Server");
        }

        /// <summary>
        /// Gets the sender's name from the network packet by referecing the name table
        /// </summary>
        /// <param name="packet">NetPacket containing the header</param>
        /// <returns>Sender's name</returns>
        public string GetSenderName(NetPacket packet)
        {            
            return this._clientNames[packet.Header.Source];
        }

        /// <summary>
        /// Gets the receiver's name from the network packet by referecing the name table
        /// </summary>
        /// <param name="packet">NetPacket containing the header</param>
        /// <returns>Sender's name</returns>
        public string GetReceiverName(NetPacket packet)
        {            
            return this._clientNames[packet.Header.Destination];
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        /// <param name="port">Port to listen to</param>
        /// <returns>True if the server has started</returns>
        public bool Start(int port)
        {
            try
            {                
                /* Initializes the Listener */
                this._tcpListener = new TcpListener(IPAddress.Any, port);

                /* Start Listeneting at the specified port */
                this._tcpListener.Start();
                IPHostEntry serverIPs = Dns.GetHostEntry(Dns.GetHostName());
                this._serverIP = null;
                foreach (IPAddress ip in serverIPs.AddressList)
                {
                    this._serverIP = ip.ToString();
                    if (this._serverIP.IndexOf("192.168.") >= 0)
                        break;
                    this._serverIP = null;
                }
                
                if (this.OnServerStart != null)
                {
                    String status =    "Server is running at socket: " + this._serverIP +":"+port+"\r\n"+
                                        "The local End point is: " + this._tcpListener.LocalEndpoint;                                    

                    this.OnServerStart(this._tcpListener, new NetEventArgs(this._tcpListener.LocalEndpoint, status));
                    this.WaitForClient();   // Start waiting for clients
                }
                this._isRunning = true;
                return true;
            }
            catch (Exception e)
            {
                if (this.OnError != null)
                    this.OnError(this._tcpListener, new NetEventArgs(e));
                return false;
            }
        }

        /// <summary>
        /// Stops the server
        /// </summary>
        /// <returns>True if the server was stopped successfully</returns>
        public bool Stop()
        {
            if (this._isRunning)
            {
                try
                {
                    this._isRunning = false;

                    foreach (TcpClientManager mgr in this._clientManager.Values)
                        mgr.Stop();

                    this._tcpListener.Stop();

                    if (this.OnServerStop != null)                    
                        this.OnServerStop(this._tcpListener, new NetEventArgs("Server Stopped."));
                    
                    return true;
                }
                catch (Exception e)
                {
                    if (this.OnError != null)
                        this.OnError(this._tcpListener, new NetEventArgs(e));
                    return false;
                }
            }
            else
            {
                if (this.OnStatusChanged != null)
                    this.OnStatusChanged(this._tcpListener, new NetEventArgs("Server not started."));                
                return false;
            }
        }



        /// <summary>
        /// Drops the client
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <returns>True if the client was dropped successfully</returns>
        public bool Drop(int id)
        {
            if (this._isRunning)
            {
                try
                {
                    this._clientManager[id].Stop();
                    return true;
                }
                catch (Exception e)
                {
                    if (this.OnError != null)
                        this.OnError(this._tcpListener, new NetEventArgs(e));
                    return false;
                }
            }
            else
            {
                if (this.OnStatusChanged != null)
                    this.OnStatusChanged(this._tcpListener, new NetEventArgs("Dropped."));
                return false;
            }
        }

        /// <summary>
        /// Asynchronous waiting for client
        /// </summary>
        protected void WaitForClient()
        {

            if (this.OnStatusChanged != null)
            {
                this.OnStatusChanged(this._tcpListener, new NetEventArgs("Waiting for a connection."));
            }
            try
            {
                this._tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), this._tcpListener);
            }
            catch (Exception ex) 
            {
                String s = ex.ToString();
            }
        }

        /// <summary>
        /// Callback for client wait
        /// </summary>        
        protected void ClientConnected(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener)ar.AsyncState;                
                if (this._isRunning)
                {
                    System.Net.Sockets.TcpClient client = listener.EndAcceptTcpClient(ar);
                    // Create a manager for client
                    this.__clientCounter++;
                    TcpClientManager cm = new TcpClientManager(this.__clientCounter, client, this._serverIP);
                    cm.InvokeServer += new TcpClientManager.InvokeServerHandler(cm_InvokeServer);
                    if (this.OnDataReceived != null)
                        cm.OnClientDataReceived += new TcpClientManager.CommunicationStatusEventHandler(this.OnDataReceived);
                    if (this.OnAcknowledged != null)
                        cm.OnClientAcknowledged += new TcpClientManager.CommunicationStatusEventHandler(this.OnAcknowledged);

                    // Add to the list of clients
                    this._clientManager.Add(this.__clientCounter, cm);
                    this._clientNames.Add(this.__clientCounter, this._clientManager.Count.ToString());
                    this.DispatchNameTable();
                    if (this.OnClientConnected != null)
                        this.OnClientConnected(this._tcpListener, new NetEventArgs(client.Client.RemoteEndPoint, "Client Connected: " + client.Client.RemoteEndPoint, new NetPacket(this._clientManager.Count,0,null)));
                }                
            }
            catch (Exception e)
            {
                if (this.OnError != null)
                    this.OnError(this._tcpListener, new NetEventArgs(e));
            }
            finally     // Restart listening
            {
                if(this._isRunning)
                    this._tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), this._tcpListener);
            }
        }

        /// <summary>
        /// Event handler whenever client manager requires a server operation
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="cmd">TcpCommand operation</param>
        protected void cm_InvokeServer(int id, TcpCommand cmd)
        {
            switch (cmd.Type)
            {
                case TcpCommand.Command.Name:
                    string prev = this._clientNames[id];
                    this._clientNames[id] = UTF8Encoding.UTF8.GetString(cmd.Data);
                    this.DispatchNameTable();
                    if (this.OnStatusChanged != null)
                        this.OnStatusChanged(this._tcpListener, new NetEventArgs(String.Format("{0} is now known as {1}", prev, this._clientNames[id])));
                    if (this.OnClientStatusChanged != null)
                        this.OnClientStatusChanged(id, this._clientNames[id], cmd);
                    break;

                case TcpCommand.Command.NameTable:
                    BinaryFormatter bf = new BinaryFormatter();                    
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, this._clientNames);
                        this._clientManager[id - 1].BeginWriteToClient(0, NetPacket.PacketType.Command, new TcpCommand(true, cmd.Type, ms.ToArray()).Serialize());
                    }
                    if (this.OnClientStatusChanged != null)
                        this.OnClientStatusChanged(id, this._clientNames[id], cmd);
                    break;

                case TcpCommand.Command.Disconnect:
                    this._clientManager.Remove(id);
                    string n = this._clientNames[id];
                    this._clientNames.Remove(id);
                    if (this.OnStatusChanged != null)                    
                        this.OnStatusChanged(this._tcpListener, new NetEventArgs(String.Format("{0} Disconnected. Reason: {1}", n, UTF8Encoding.UTF8.GetString(cmd.Data))));
                    if (this.OnClientStatusChanged != null)
                        this.OnClientStatusChanged(id, n, cmd);                    
                    break;
            }
        }

        /// <summary>
        /// Dispatches the name table to each client
        /// </summary>
        protected void DispatchNameTable()
        {
             BinaryFormatter bf = new BinaryFormatter();
             using (MemoryStream ms = new MemoryStream())
             {
                 bf.Serialize(ms, this._clientNames);
                 foreach (TcpClientManager mgr in this._clientManager.Values)
                     mgr.BeginWriteToClient(0, NetPacket.PacketType.Command, new TcpCommand(true, TcpCommand.Command.NameTable, ms.ToArray()).Serialize());
             }
        }
   
        /// <summary>
        /// Re-attaches events to each client manager
        /// </summary>
        public void RefreshEvents()
        {
            foreach (TcpClientManager cm in this._clientManager.Values)
            {
                if (this.OnDataReceived != null)
                    cm.OnClientDataReceived += new TcpClientManager.CommunicationStatusEventHandler(this.OnDataReceived);
                if (this.OnAcknowledged != null)
                    cm.OnClientAcknowledged += new TcpClientManager.CommunicationStatusEventHandler(this.OnAcknowledged);
            }
        }

        /// <summary>
        /// Performs a write to client using the service of client manager
        /// </summary>
        /// <param name="dest">Client Id</param>
        /// <param name="ct">NetPacket command type</param>
        /// <param name="data">Data bytes</param>
        public void BeginWriteToClient(int dest, NetPacket.PacketType ct, byte[] data)
        {
            this._clientManager[dest].BeginWriteToClient(0, ct, data);
        }
    }
}
