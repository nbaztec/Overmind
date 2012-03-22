using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace NX.Net
{
    class TcpClientManager
    {
        #region Events and Delegates

        public delegate void CommunicationStatusEventHandler(object sender, NetEventArgs e);
        public event CommunicationStatusEventHandler OnClientDataReceived;
        public event CommunicationStatusEventHandler OnClientAcknowledged;

        #endregion

        #region Variables

        private System.Net.Sockets.TcpClient client;
        private UTF8Encoding utfEncoder = new UTF8Encoding();
        private String serverIP;

        #endregion

        #region Constructor

        public TcpClientManager(System.Net.Sockets.TcpClient client, String serverIP)
        {
            this.client = client;
            this.BeginReadFromClient();
            this.serverIP = serverIP;
        }

        #endregion

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
            if (this.client.Connected)
            {
                this.msgLen = new byte[4];
                System.Net.Sockets.NetworkStream clientStream = this.client.GetStream();
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
            if (this.OnClientDataReceived != null)
            {
                NetEventArgs e = new NetEventArgs(packet);
                if (this.OnClientDataReceived != null)
                    this.OnClientDataReceived(this.client, e);
                //if (e.sendAcknowledgement)
                this.BeginWriteToClient(new NetPacket(NetPacket.CommandType.Print, "Received Data", new NetPacket.Entity(this.serverIP,"server"),packet.From));
            }
        }

        #endregion

        #region Multi-Threaded Synchronous Writer

        private void BeginWriteToClient(NetPacket packet)
        {
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
            System.Net.Sockets.NetworkStream clientStream = this.client.GetStream();
            clientStream.Write(bytePack, 0, bytePack.Length);
            if (this.OnClientAcknowledged != null)
                this.OnClientAcknowledged(this.client, new NetEventArgs("ack"));
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
