using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace NX.Net
{
    /// <summary>
    /// Manages individual clients
    /// </summary>
    class TcpClientManager
    {
        #region Events and Delegates

        /// <summary>
        /// Delegate for raising event related to client communication
        /// </summary>        
        public delegate void CommunicationStatusEventHandler(object sender, NetEventArgs e);       
        public event CommunicationStatusEventHandler OnClientDataReceived;
        public event CommunicationStatusEventHandler OnClientAcknowledged;

        /// <summary>
        /// Delegate for requesting server involvement
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="cmd">TcpCommand to be processed</param>
        public delegate void InvokeServerHandler(int id, TcpCommand cmd);
        public event InvokeServerHandler InvokeServer;

        #endregion

        #region Variables

        private int _clientId;
        private System.Net.Sockets.TcpClient _tcpClient;
        private UTF8Encoding utfEncoder = new UTF8Encoding();
        private String serverIP;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientId">Client Id</param>
        /// <param name="client">TcpClient instance for the client</param>
        /// <param name="serverIP">Server's IP</param>
        public TcpClientManager(int clientId, System.Net.Sockets.TcpClient client, String serverIP)
        {
            this._clientId = clientId;
            this._tcpClient = client;            
            this.BeginReadFromClient();
            this.serverIP = serverIP;
        }

        #endregion

        /// <summary>
        /// Stop client manager and disconnect the client
        /// </summary>
        public void Stop()
        {
            this.StartWriteSync(new NetPacket(0, this._clientId, NetPacket.PacketType.Command, (new TcpCommand(true, TcpCommand.Command.Shutdown)).Serialize()));            
            this._tcpClient.Close();
        }

        /// <summary>
        /// Parses the TcpCommand received by the client
        /// </summary>
        /// <param name="bytes">Data bytes</param>
        private void ParseCommand(byte[] bytes)
        {
            TcpCommand cmd = new TcpCommand();
            cmd.Deserialize(bytes);

            if (cmd.IsRequest)    // Means a client request
            {
                switch (cmd.Type)
                {
                    case TcpCommand.Command.Acknowledge:                            // Client needs acknowledgement     
                        TcpCommand response = new TcpCommand(true, TcpCommand.Command.Acknowledge);
                        byte[] ack_id = response.Serialize();
                        using (MemoryStream rms = new MemoryStream())
                        {
                            byte[] _t = BitConverter.GetBytes(this._clientId);      // Acknowledge and tell the client his Id
                            rms.Write(ack_id, 0, ack_id.Length);            
                            rms.Write(_t, 0, _t.Length);
                            // Send Acknowledgement and Id
                            this.BeginWriteToClient(this._clientId, NetPacket.PacketType.Command, rms.ToArray());
                        }
                        if (this.OnClientAcknowledged != null)
                            this.OnClientAcknowledged(this._tcpClient, new NetEventArgs(new NetPacket(0, this._clientId, null), "ack"));
                        if (cmd.Data != null)
                            this.InvokeServer(this._clientId, new TcpCommand(TcpCommand.Command.Name, cmd.Data));                        
                        break;

                    case TcpCommand.Command.Name:                                   // Client requests name change
                        this.InvokeServer(this._clientId, cmd);
                        break;

                    case TcpCommand.Command.Disconnect:                             // Client requests name table
                        this.InvokeServer(this._clientId, cmd);
                        break;
                }
            }
                        
        }

        #region Multi-Threaded Asynchronous Reader

        /// <summary>
        /// Start reading from client
        /// </summary>
        public void BeginReadFromClient()
        {
            Thread t = new Thread(this.StartReadThread);
            t.Name = "ServerStartReadThread";
            t.Start();
        }

        private byte[] __dataBuffer = null;
        /// <summary>
        /// Start read async read operation
        /// </summary>
        private void StartReadThread()
        {
            try
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
            catch (Exception) { }
        }        

        /// <summary>
        /// End async read operation
        /// </summary>        
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
                        {
                            offset += ns.Read(data, offset, dataLen - offset);
                        }

                        //ns.Read(data, 0, dataLen);
                        Thread t = new Thread(this.UnsealPacket);
                        t.Name = "ServerUnsealPacket";
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
                    if (this._tcpClient.Client.Connected)
                        this.BeginReadFromClient();
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Deserialize NetPacket
        /// </summary>
        /// <param name="obj">Data bytes</param>
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
                    if (this.OnClientDataReceived != null)
                    {
                        NetEventArgs e = new NetEventArgs(packet);
                        this.OnClientDataReceived(this._tcpClient, e);
                    }                    
                    break;
            }
        }

        #endregion

        #region Multi-Threaded Synchronous Writer
        
        /// <summary>
        /// Begin async write from client
        /// </summary>
        /// <param name="src">Receiver's Id</param>
        /// <param name="ct">NetPacket command type</param>
        /// <param name="data">Data bytes</param>
        public void BeginWriteToClient(int src, NetPacket.PacketType ct, byte[] data)
        {
            NetPacket packet = new NetPacket(src, this._clientId, ct, data);
            Thread t = new Thread(this.StartWriteSync);
            t.Name = "ServerStartWriteSync";
            t.Start(packet);
        }

        /// <summary>
        /// Start writing to stream
        /// </summary>
        /// <param name="obj">NetPacket object</param>
        private void StartWriteSync(object obj)
        {
            byte[] bytePack = this.SealPacket((obj as NetPacket).Serialize());
            System.Net.Sockets.NetworkStream clientStream = this._tcpClient.GetStream();
            if (clientStream.CanWrite)
                clientStream.Write(bytePack, 0, bytePack.Length);
        }

        /// <summary>
        /// Serialize NetPacket by adding length header
        /// </summary>
        /// <param name="oBytes">Data bytes of NetPacket object</param>
        /// <returns>Data bytes for transmission</returns>
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
