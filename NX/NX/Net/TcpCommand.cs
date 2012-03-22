using System;
using System.Collections.Generic;
using System.Text;

namespace NX.Net
{
    public class TcpCommand : IBinarySerializer
    {
        /// <summary>
        /// Type of Tcp Command, internal to functioning of server-client
        /// </summary>
        public enum Command : byte      // Use 7-bits only. 8th bit for Respose/Request flag
        {
            // Client Commands
            None = 0x00,        // Empty
            Disconnect,         // Disconnect client.
            Acknowledge,        // Acknowledge connection
            Ping,               // Ping
            Name,               // Change name            
            NameTable,          // Get the client name table   
        
            // Server Commands                           
            Shutdown = 0x7F     // Shutdown
        }

        private Command _cmd;
        private bool _resp = false;
        private byte[] _data = null;

        public Command Type { get { return this._cmd; } }        
        public bool IsResponse { get { return this._resp; } }        
        public bool IsRequest { get { return !this._resp; } }
        public byte[] Data { get { return this._data; } }

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public TcpCommand()
        {
            this._cmd = Command.None;
            this._resp = false;
            this._data = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cmd">Command type</param>
        public TcpCommand(Command cmd)
            : this()
        {
            this._cmd = cmd;            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cmd">Command type</param>
        /// <param name="data">Data associated</param>
        public TcpCommand(Command cmd, byte[] data)
            : this()
        {
            this._cmd = cmd;            
            this._data = data;
        }  

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response">Whether command is a response</param>
        /// <param name="cmd">Command type</param>
        public TcpCommand(bool response, Command cmd)
            :this()
        {
            this._cmd = cmd;
            this._resp = response;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response">Whether command is a response</param>
        /// <param name="cmd">Command type</param>
        /// <param name="data">Data associated</param>
        public TcpCommand(bool response, Command cmd, byte[] data)            
        {
            this._cmd = cmd;
            this._resp = response;
            this._data = data;
        }

        #endregion

        #region Interface Methods

        /// <summary>
        /// Serialize TcpCommand object
        /// </summary>
        /// <returns>Data bytes</returns>
        public byte[] Serialize()
        {
            byte data = (byte)this._cmd;
            data |= (byte)(this._resp ? 0x80 : 0x00);
            byte[] cmdBytes = {data};
            if (this._data != null)
            {
                byte[] _t = new byte[sizeof(byte) + this._data.LongLength];                
                _t[0] = cmdBytes[0];
                for (long i = 0; i < this._data.LongLength; i++)
                    _t[1+i] = this._data[i];
                cmdBytes = _t;
            }
            return cmdBytes;
        }

        /// <summary>
        /// Deserialize TcpCommand object
        /// </summary>
        /// <param name="bytes">Data bytes</param>
        /// <returns>Length of bytes read</returns>
        public long Deserialize(byte[] bytes)
        {
            byte data = bytes[0];
            this._resp = (data & 0x80) == 0x80;
            data &= 0x7F;
            this._cmd = (Command)data;
            
            if(bytes.LongLength == sizeof(byte))    // If no data
                return sizeof(byte);

            this._data = new byte[bytes.LongLength - sizeof(byte)];
            for (long i = 1; i < bytes.Length; i++)
                this._data[i-1] = bytes[i];
            return sizeof(byte) + this._data.LongLength;
        }

        #endregion

    }
}
