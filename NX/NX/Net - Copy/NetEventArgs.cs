using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NX.Net
{
    public class NetEventArgs
    {
        private readonly EndPoint _endPoint;        
        private readonly string _msg;
        private readonly NetPacket _packet;
        private readonly Exception _exception;

        public EndPoint EndPoint { get { return this._endPoint; } }
        public string Message { get { return this._msg; } }
        public NetPacket Packet { get { return this._packet; } }
        public Exception Exception { get { return this._exception; } }


        public NetEventArgs(EndPoint endPoint, string msg, NetPacket _packet, Exception exception)
        {
            this._endPoint = endPoint;
            this._msg = msg;
            this._packet = _packet;
            this._exception = exception;
        }

        public NetEventArgs(EndPoint endPoint)
            : this(endPoint, null, null, null)
        {
        }

        public NetEventArgs(NetPacket packet)
            : this(null, null, packet, null)
        {            
        }

        public NetEventArgs(string msg)
            : this(null, msg, null, null)
        {
        }

        public NetEventArgs(Exception exception)
            : this(null, null, null, exception)
        {
        }
        /*public NetEventArgs(Exception exception)
            : this(null, null, null, exception)
        {
        }*/

        public NetEventArgs(EndPoint endPoint, Exception exception)            
            : this(endPoint)
        {            
            this._exception = exception;
        }

        public NetEventArgs(EndPoint endPoint, string msg)
            : this(endPoint)
        {
            this._msg = msg;
        }

        public NetEventArgs(EndPoint endPoint, string msg, NetPacket packet)
            : this(endPoint, msg)
        {
            this._packet = packet;
        }

        public NetEventArgs(NetPacket packet, string msg)
            : this(packet)
        {
            this._msg = msg;
        }
    }
}
