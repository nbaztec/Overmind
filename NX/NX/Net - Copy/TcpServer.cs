using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace NX.Net
{
    public class TcpServer
    {
        TcpListener tcpListener = null;        
        UTF8Encoding utfEncoder = new UTF8Encoding();
        List<TcpClientManager> clientManager = new List<TcpClientManager>(10);
        private bool isRunning = false;
        private String serverIP;

        public delegate void ServerStatusEventHandler(object sender, NetEventArgs e);
        
        public event ServerStatusEventHandler OnStatusChanged;
        public event ServerStatusEventHandler OnServerStart;
        public event ServerStatusEventHandler OnServerStop;
        public event ServerStatusEventHandler OnClientConnected;
        public event ServerStatusEventHandler OnDataReceived;
        public event ServerStatusEventHandler OnAcknowledged;        
        public event ServerStatusEventHandler OnError;

        public TcpServer()
        {
           
        }

        public bool Start(int port)
        {
            try
            {                
                /* Initializes the Listener */
                this.tcpListener = new TcpListener(IPAddress.Any, port);

                /* Start Listeneting at the specified port */
                this.tcpListener.Start();
                IPHostEntry serverIPs = Dns.GetHostEntry(Dns.GetHostName());
                this.serverIP = null;
                foreach (IPAddress ip in serverIPs.AddressList)
                {
                    this.serverIP = ip.ToString();
                    if (this.serverIP.IndexOf("192.168.") >= 0)
                        break;
                    this.serverIP = null;
                }
                
                if (this.OnServerStart != null)
                {
                    String status =    "Server is running at socket: " + this.serverIP +":"+port+"\r\n"+
                                        "The local End point is: " + this.tcpListener.LocalEndpoint;                                    

                    this.OnServerStart(this.tcpListener, new NetEventArgs(this.tcpListener.LocalEndpoint, status));
                }
                this.isRunning = true;
                return true;
            }
            catch (Exception e)
            {
                if (this.OnError != null)
                    this.OnError(this.tcpListener, new NetEventArgs(e));
                return false;
            }
        }

        public bool Stop()
        {
            if (this.isRunning)
            {
                try
                {
                    this.tcpListener.Stop();
                    if (this.OnServerStop != null)
                    {
                        this.OnServerStop(this.tcpListener, new NetEventArgs("Server Stopped."));
                    }
                    this.isRunning = false;
                    return true;
                }
                catch (Exception e)
                {
                    if (this.OnError != null)
                        this.OnError(this.tcpListener, new NetEventArgs(e));
                    return false;
                }
            }
            else
            {
                if (this.OnStatusChanged != null)
                    this.OnStatusChanged(this.tcpListener, new NetEventArgs("Server not started."));                
                return false;
            }
        }

        public void WaitForClient()
        {

            if (this.OnStatusChanged != null)
            {
                this.OnStatusChanged(this.tcpListener, new NetEventArgs("Waiting for a connection."));
            }
            try
            {
                this.tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), this.tcpListener);
            }
            catch (Exception ex) 
            {
                String s = ex.ToString();
            }
        }

        private void ClientConnected(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener)ar.AsyncState;
                System.Net.Sockets.TcpClient client = listener.EndAcceptTcpClient(ar);

                //Add To Client List
                TcpClientManager cm = new TcpClientManager(client, this.serverIP);
                if(this.OnDataReceived != null)
                    cm.OnClientDataReceived += new TcpClientManager.CommunicationStatusEventHandler(this.OnDataReceived);
                if(this.OnAcknowledged != null)
                    cm.OnClientAcknowledged += new TcpClientManager.CommunicationStatusEventHandler(this.OnAcknowledged);
                
                //Add
                this.clientManager.Add(cm);

                if (this.OnClientConnected != null)
                {
                    this.OnClientConnected(this.tcpListener, new NetEventArgs(client.Client.RemoteEndPoint, "Client Connected: " + client.Client.RemoteEndPoint));
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("Error: " + e);
                //Console.Read();
                if (this.OnError != null)
                    this.OnError(this.tcpListener, new NetEventArgs(e));
            }
            finally
            {
                this.tcpListener.BeginAcceptTcpClient(new AsyncCallback(ClientConnected), this.tcpListener);
            }
        }    
   
        public void RefreshEvents()
        {
            foreach (TcpClientManager cm in this.clientManager)
            {
                if (this.OnDataReceived != null)
                    cm.OnClientDataReceived += new TcpClientManager.CommunicationStatusEventHandler(this.OnDataReceived);
                if (this.OnAcknowledged != null)
                    cm.OnClientAcknowledged += new TcpClientManager.CommunicationStatusEventHandler(this.OnAcknowledged);
            }
        }
    }
}
