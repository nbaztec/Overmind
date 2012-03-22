using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace NX.Net
{
    public class TcpClient
    {
        System.Net.Sockets.TcpClient tcpClient = null;        
        UTF8Encoding utfEncoder = new UTF8Encoding();        
        NetPacket.Entity clientEntity = null;

        public delegate void ClientStatusEventHandler(object sender, NetEventArgs e);
        //public event ClientStatusEventHandler onReceive;
        public event ClientStatusEventHandler OnStatusChanged;        
        public event ClientStatusEventHandler OnDataReceived;
        public event ClientStatusEventHandler OnConnectionSuccess;
        public event ClientStatusEventHandler OnError;

        public TcpClient()
        {
            this.tcpClient = new System.Net.Sockets.TcpClient();
        }

        public void Connect(String ip, int port)
        {
            try
            {
                if (this.tcpClient == null)
                    this.tcpClient = new System.Net.Sockets.TcpClient();
                
                if (this.tcpClient.Connected)
                {
                    if (this.OnStatusChanged != null)
                        this.OnStatusChanged(this.tcpClient, new NetEventArgs("Already connected."));
                }
                else
                {
                    //this.tcpClient = new TcpClient();
                    this.tcpClient.BeginConnect(ip, port, new AsyncCallback(ClientConnected), this.tcpClient);
                    if (this.OnStatusChanged != null)
                        this.OnStatusChanged(this.tcpClient, new NetEventArgs("Attempting to connect."));
                }
            }
            catch (Exception e)
            {
                if (this.OnError != null)
                    this.OnError(this.tcpClient, new NetEventArgs(e));
            }
        }

        public void Disconnect()
        {
            if (this.tcpClient != null && this.tcpClient.Connected)
            {
                try
                {
                    this.tcpClient.Close();
                    this.tcpClient = null;
                }
                catch (Exception) { }
                if (this.OnStatusChanged != null)
                    this.OnStatusChanged(this.tcpClient, new NetEventArgs("Client Disconnected."));
                //this.tcpClient = new TcpClient();
            }
            else if (this.OnStatusChanged != null)
                this.OnStatusChanged(this.tcpClient, new NetEventArgs("No connection has been established."));
        }

        private void ClientConnected(IAsyncResult ar)
        {
            try
            {
                if (this.OnConnectionSuccess != null)
                    this.OnConnectionSuccess(this.tcpClient, new NetEventArgs(this.tcpClient.Client.RemoteEndPoint, "Connection Success"));
                
                System.Net.Sockets.TcpClient client = (System.Net.Sockets.TcpClient)ar.AsyncState;
                client.EndConnect(ar);                
                this.clientEntity = new NetPacket.Entity(this.tcpClient.Client.RemoteEndPoint.ToString(), "Client");
                /*byte[] msg = this.utfEncoder.GetBytes("I have arrived");
                this.nStream.BeginWrite(msg, 0, msg.Length, null, null);*/
                this.BeginWriteToClient(new NetPacket(NetPacket.CommandType.Print, "I have arrived", this.clientEntity, new NetPacket.Entity("","server")));
                this.BeginReadFromClient();
            }
            catch (Exception e)
            {
                if (this.OnError != null)
                    this.OnError(this.tcpClient, new NetEventArgs(e));
            }
        }

        #region Multi-Threaded Asynchronous Reader

        private byte[] msgLen = null;

        private void EndReadFromClient(IAsyncResult ar)
        {
            try
            {
                System.Net.Sockets.NetworkStream ns = ar.AsyncState as System.Net.Sockets.NetworkStream;
                int len = ns.EndRead(ar);
                if (len == 4)
                {
                    int dataLen = BitConverter.ToInt32(this.msgLen, 0);
                    byte[] data = new byte[dataLen];
                    ns.Read(data, 0, dataLen);
                    Thread t = new Thread(this.ParseReceivedData);
                    t.Start(data);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                this.BeginReadFromClient();
            }
        }

        private void StartReadThread()
        {
            if (this.tcpClient != null && this.tcpClient.Connected)
            {
                this.msgLen = new byte[4];
                System.Net.Sockets.NetworkStream clientStream = this.tcpClient.GetStream();
                clientStream.BeginRead(msgLen, 0, 4, new AsyncCallback(this.EndReadFromClient), clientStream);
            }
        }

        private void BeginReadFromClient()
        {
            Thread t = new Thread(this.StartReadThread);
            t.Start();
        }

        #endregion

        #region Data Deserializer

        private void ParseReceivedData(object obj)
        {
            byte[] oBytes = obj as byte[];
            MemoryStream mStream = new MemoryStream(oBytes);
            mStream.Position = 0;
            BinaryFormatter bFormat = new BinaryFormatter();
            NetPacket packet = bFormat.Deserialize(mStream) as NetPacket;
            if (this.OnDataReceived != null)
            {
                NetEventArgs e = new NetEventArgs(packet);
                this.OnDataReceived(this.tcpClient, e);                
            }
        }

        #endregion

        #region Multi-Threaded Synchronous Writer

        public void BeginWriteToClient(NetPacket packet)
        {
            Thread t = new Thread(this.StartWriteThread);
            t.Start(packet);
        }

        public void BeginWriteToClient(NetPacket.CommandType ct, String text)
        {
            NetPacket packet = new NetPacket(NetPacket.CommandType.Print, text, this.clientEntity, new NetPacket.Entity("","server"));
            Thread t = new Thread(this.StartWriteThread);
            t.Start(packet);
        }

        private void StartWriteThread(object obj)
        {
            NetPacket packet = obj as NetPacket;
            MemoryStream mStream = new MemoryStream();
            BinaryFormatter bFormat = new BinaryFormatter();

            bFormat.Serialize(mStream, packet);
            byte[] bytePack = this.MakeFinalPacket(mStream.ToArray());
            System.Net.Sockets.NetworkStream clientStream = this.tcpClient.GetStream();
            clientStream.Write(bytePack, 0, bytePack.Length);
            /*if (this.onClientAcknowledged != null)
                this.onClientAcknowledged(this.client, new NetEventArgs("Acknowledged"));*/
        }

        private byte[] MakeFinalPacket(byte[] oBytes)
        {
            MemoryStream myStream = new MemoryStream();
            myStream.Position = 0;
            myStream.Write(BitConverter.GetBytes(oBytes.Length), 0, 4);
            myStream.Write(oBytes, 0, oBytes.Length);
            return myStream.ToArray();
        }

        #endregion
    }
}
