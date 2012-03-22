using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NX.Net
{
    /// <summary>
    /// Manages arguments in raised network events
    /// </summary>
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

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Specifies endpoint of sender</param>
        /// <param name="msg">Message associated with event</param>
        /// <param name="_packet">Packet received</param>
        /// <param name="exception">Any exception, if occured</param>
        public NetEventArgs(EndPoint endPoint, string msg, NetPacket _packet, Exception exception)
        {
            this._endPoint = endPoint;
            this._msg = msg;
            this._packet = _packet;
            this._exception = exception;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Specifies endpoint of sender</param>
        public NetEventArgs(EndPoint endPoint)
            : this(endPoint, null, null, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>        
        /// <param name="_packet">Packet received</param>        
        public NetEventArgs(NetPacket packet)
            : this(null, null, packet, null)
        {            
        }

        /// <summary>
        /// Constructor
        /// </summary>        
        /// <param name="msg">Message associated with event</param>        
        public NetEventArgs(string msg)
            : this(null, msg, null, null)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>        
        /// <param name="exception">Any exception, if occured</param>
        public NetEventArgs(Exception exception)
            : this(null, null, null, exception)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Specifies endpoint of sender</param>        
        /// <param name="exception">Any exception, if occured</param>
        public NetEventArgs(EndPoint endPoint, Exception exception)            
            : this(endPoint)
        {            
            this._exception = exception;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Specifies endpoint of sender</param>
        /// <param name="msg">Message associated with event</param>        
        public NetEventArgs(EndPoint endPoint, string msg)
            : this(endPoint)
        {
            this._msg = msg;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Specifies endpoint of sender</param>
        /// <param name="msg">Message associated with event</param>
        /// <param name="_packet">Packet received</param>        
        public NetEventArgs(EndPoint endPoint, string msg, NetPacket packet)
            : this(endPoint, msg)
        {
            this._packet = packet;
        }

        /// <summary>
        /// Constructor
        /// </summary>        
        /// <param name="msg">Message associated with event</param>
        /// <param name="_packet">Packet received</param>        
        public NetEventArgs(NetPacket packet, string msg)
            : this(packet)
        {
            this._msg = msg;
        }

        #endregion
    }
}
