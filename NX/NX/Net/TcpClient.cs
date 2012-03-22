using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace NX.Net
{
    /// <summary>
    /// Manages a Tcp client connection
    /// </summary>
    public class TcpClient
    {
        System.Net.Sockets.TcpClient _tcpClient = null;        
        UTF8Encoding utfEncoder = new UTF8Encoding();        

        private int _id = -1;
        private string _name = "";
        private bool _clientAcknowledged = false;
        public bool Acknowledged {
            get 
            { 
                return this._clientAcknowledged; 
            }
        }

        /// <summary>
        /// Delegate for raising events whenever client status changes
        /// </summary>        
        public delegate void ClientStatusEventHandler(object sender, NetEventArgs e);
        //public event ClientStatusEventHandler onReceive;
        public event ClientStatusEventHandler OnStatusChanged;        
        public event ClientStatusEventHandler OnDataReceived;
        public event ClientStatusEventHandler OnConnectionSuccess;
        public event ClientStatusEventHandler OnDisconnect;
        public event ClientStatusEventHandler OnError;

        /// <summary>
        /// Local copy of the name table containing client names
        /// </summary>
        private Dictionary<int, string> _clientNames = new Dictionary<int, string>();

        public bool Connected
        {
            get
            {
                try
                {
                    return this._tcpClient.Connected;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public TcpClient()
        {
            this._tcpClient = new System.Net.Sockets.TcpClient();
        }

        #endregion

        /// <summary>
        /// Gets sender's name
        /// </summary>
        /// <param name="packet">NetPacket object</param>
        /// <returns>Name of the sender</returns>
        public string GetSenderName(NetPacket packet)
        {
            return this._clientNames[packet.Header.Source];
        }

        /// <summary>
        /// Connects to the TcpServer
        /// </summary>
        /// <param name="ip">Server IP</param>
        /// <param name="port">Connection Port</param>
        public void Connect(String ip, int port)
        {
            try
            {
                if (this._tcpClient == null)
                    this._tcpClient = new System.Net.Sockets.TcpClient();
                
                if (this._tcpClient.Connected)
                {
                    if (this.OnStatusChanged != null)
                        this.OnStatusChanged(this._tcpClient, new NetEventArgs("Already connected."));
                }
                else
                {
                    //this.tcpClient = new TcpClient();
                    this._tcpClient.BeginConnect(ip, port, new AsyncCallback(ClientConnected), this._tcpClient);
                    if (this.OnStatusChanged != null)
                        this.OnStatusChanged(this._tcpClient, new NetEventArgs("Attempting to connect."));
                }
            }
            catch (Exception e)
            {
                if (this.OnError != null)
                    this.OnError(this._tcpClient, new NetEventArgs(e));
            }
        }

        /// <summary>
        /// Connect to the server and sets client's name
        /// </summary>
        /// <param name="ip">Server IP</param>
        /// <param name="port">Connection Port</param>
        /// <param name="name">Client's name</param>
        public void Connect(String ip, int port, string name)
        {
            this._name = name;
            this.Connect(ip, port);
        }

        /// <summary>
        /// Callback for client's connection
        /// </summary>        
        private void ClientConnected(IAsyncResult ar)
        {
            try
            {
                System.Net.Sockets.TcpClient client = (System.Net.Sockets.TcpClient)ar.AsyncState;
                client.EndConnect(ar);

                // Request Acknowledgement
                this.StartWriteSync(new NetPacket(0, this._id, NetPacket.PacketType.Command, new TcpCommand(TcpCommand.Command.Acknowledge, UnicodeEncoding.UTF8.GetBytes(this._name)).Serialize()));
                this.BeginReadFromClient();
            }
            catch (Exception e)
            {
                if (this.OnError != null)
                    this.OnError(this._tcpClient, new NetEventArgs(e));
            }
        }

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        /// <param name="reason">Reason for disconnection</param>
        public void Disconnect(string reason)
        {
            if (this._tcpClient != null && this._tcpClient.Connected)
            {
                try
                {                    
                    this.StartWriteSync(new NetPacket(this._id, 0, NetPacket.PacketType.Command, new TcpCommand(TcpCommand.Command.Disconnect, UnicodeEncoding.UTF8.GetBytes(reason)).Serialize()));
                    this._tcpClient.Close();
                    this._tcpClient = null;

                    if (this.OnDisconnect != null)
                        this.OnDisconnect(this._tcpClient, new NetEventArgs("Client Disconnected. " + reason));
                }
                catch (Exception) { }                
                //this.tcpClient = new TcpClient();
            }
            else if (this.OnStatusChanged != null)
                this.OnStatusChanged(this._tcpClient, new NetEventArgs("No connection has been established."));
        }        
        
        /// <summary>
        /// Parses the TcpCommand received from the server
        /// </summary>
        /// <param name="bytes">Data bytes</param>
        private void ParseCommand(byte[] bytes)
        {
            TcpCommand cmd = new TcpCommand();
            cmd.Deserialize(bytes);

            if (cmd.IsResponse)                                                     // Server responses
            {
                switch (cmd.Type)
                {
                    case TcpCommand.Command.Acknowledge:
                        this._clientAcknowledged = true;
                        this._id = BitConverter.ToInt32(cmd.Data, 0);               // Get client's Id
                        if (this.OnConnectionSuccess != null && this._id != -1)     // Raise Connected event if server acknowledged
                            this.OnConnectionSuccess(this._tcpClient, new NetEventArgs(this._tcpClient.Client.RemoteEndPoint, "Connection Success"));
                        break;
                    
                    case TcpCommand.Command.Shutdown:
                        this.Disconnect("Server Shutting Down.");
                        break;

                    case TcpCommand.Command.NameTable:
                        BinaryFormatter bf = new BinaryFormatter();
                        this._clientNames = (Dictionary<int, string>)bf.Deserialize(new MemoryStream(cmd.Data));
                        break;                    
                }
            }

            /*using (MemoryStream ms = new MemoryStream(bytes))
            {
                byte[] data = new byte[sizeof(uint)];
                ms.Read(data, 0, data.Length);
                TcpCommand cmd = new TcpCommand();
                cmd.Deserialize(data);

                if (cmd.IsResponse)
                {
                    switch (cmd.Type)
                    {
                        case TcpCommand.Command.Acknowledge:
                            data = new byte[sizeof(int)];
                            ms.Read(data, 0, sizeof(int));
                            this._id = BitConverter.ToInt32(data, 0);
                            if (this.OnConnectionSuccess != null && this._id != -1) // Raise Connected
                                this.OnConnectionSuccess(this._tcpClient, new NetEventArgs(this._tcpClient.Client.RemoteEndPoint, "Connection Success"));
                            break;
                        case TcpCommand.Command.Shutdown:
                            this.Disconnect("Server Shutting Down.");
                            break;
                    }
                }
            }*/
        }

        #region Multi-Threaded Asynchronous Reader

        private void BeginReadFromClient()
        {
            Thread t = new Thread(this.StartReadThread);
            t.Name = "StartReadThread";
            t.Start();
        }

        private byte[] __dataBuffer = null;
        private void StartReadThread()
        {
            if (this._tcpClient != null && this._tcpClient.Connected)   // Read 4-bit Length Header
            {
                this.__dataBuffer = new byte[4];
                System.Net.Sockets.NetworkStream clientStream = this._tcpClient.GetStream();
                if (clientStream.CanRead)
                {
                    IAsyncResult iar = clientStream.BeginRead(__dataBuffer, 0, 4, new AsyncCallback(this.EndReadFromClient), clientStream);
                    iar.AsyncWaitHandle.WaitOne();
                }
            }
        }        

        private void EndReadFromClient(IAsyncResult ar)
        {
            try
            {
                System.Net.Sockets.NetworkStream ns = ar.AsyncState as System.Net.Sockets.NetworkStream;
                if (ns.CanRead)
                {
                    int len = ns.EndRead(ar);
                    if (len == 4)   // Look for 4-bit length header
                    {
                        int dataLen = BitConverter.ToInt32(this.__dataBuffer, 0); // Get new info
                        byte[] data = new byte[dataLen];

                        int offset = 0;
                        while (offset < dataLen)
                            offset += ns.Read(data, offset, dataLen - -offset);

                        //ns.Read(data, 0, dataLen);
                        Thread t = new Thread(this.UnsealPacket);
                        t.Name = "UnsealPacket";
                        t.Start(data);
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                try
                {
                    if (this._tcpClient.Connected)
                        this.BeginReadFromClient();
                }
                catch (Exception) { }
            }
        }

        private void UnsealPacket(object obj)
        {
            NetPacket packet = new NetPacket();
            packet.Deserialize(obj as byte[]);
            switch (packet.Header.Type)
            {
                case NetPacket.PacketType.Command:                    
                    this.ParseCommand(packet.Data);
                    break;
                default:
                    if (this.OnDataReceived != null)
                    {
                        NetEventArgs e = new NetEventArgs(packet);
                        this.OnDataReceived(this._tcpClient, e);
                    }
                    break;
            }
        }

        #endregion

        #region Multi-Threaded Synchronous Writer
        
        public void BeginWriteToClient(int dest, NetPacket.PacketType ct, byte[] data)
        {
            NetPacket packet = new NetPacket(this._id, dest, ct, data);
            Thread t = new Thread(this.StartWriteSync);
            t.Name = "StartWriteSync";
            t.Start(packet);
        }

        private void StartWriteSync(object obj)
        {
            try
            {
                byte[] bytePack = this.SealPacket((obj as NetPacket).Serialize());
                System.Net.Sockets.NetworkStream clientStream = this._tcpClient.GetStream();
                if (clientStream.CanWrite)
                {
                    clientStream.Write(bytePack, 0, bytePack.Length);
                }
            }
            catch (Exception) { }
            
        }

        private byte[] SealPacket(byte[] oBytes)
        {
            MemoryStream myStream = new MemoryStream();
            myStream.Position = 0;
            myStream.Write(BitConverter.GetBytes(oBytes.Length), 0, sizeof(int));
            myStream.Write(oBytes, 0, oBytes.Length);
            return myStream.ToArray();
        }

        #endregion
    }
}
