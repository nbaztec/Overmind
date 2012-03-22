using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NX.Net
{    
    public partial class NetPacket : IBinarySerializer
    {
        /// <summary>
        /// Packet type enum
        /// </summary>
        public enum PacketType : byte
        {
            None = 0,   // Placeholder
            Custom,     // Packet contains custom object
            Command,    // Packet contains TcpCommand, reserved for internal uses
            Print       // Packet contains message to print
            //Upload
            //Download
            //System
        }

        private PacketHeader _header;
        private byte[] _data;
        
        public PacketHeader Header { get { return this._header; } }
        public byte[] Data { get { return this._data; } }

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public NetPacket()
        {
            this._data = null;
            this._header = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="src">Source Id</param>
        /// <param name="dest">Destination Id</param>
        /// <param name="type">Packet type</param>
        /// <param name="data">Data bytes</param>
        public NetPacket(int src, int dest, PacketType type, byte[] data)
        {
            this._header = new PacketHeader(src, dest, type);
            this._data = data;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="src">Source Id</param>
        /// <param name="dest">Destination Id</param>        
        /// <param name="data">Data bytes</param>
        public NetPacket(int src, int dest, byte[] data)
        {
            this._header = new PacketHeader(src, dest, PacketType.Print);
            this._data = data;
        }

        #endregion
        
        #region Interface Methods

        /// <summary>
        /// Serialize NetPacket object
        /// </summary>
        /// <returns>Data bytes</returns>
        public byte[] Serialize()
        {            
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] header = this._header.Serialize();                                
                ms.Write(header, 0, header.Length);
                ms.Write(this._data, 0, this._data.Length);
                return ms.ToArray();
            }                        
        }

        /// <summary>
        /// Deserializes NetPacket object
        /// </summary>
        /// <param name="bytes">Data bytes</param>
        /// <returns>Length of bytes read</returns>
        public long Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                this._header = new PacketHeader();
                ms.Position = this._header.Deserialize(bytes);
                long len = ms.Length - ms.Position;                
                int buffLen = (int)Math.Min(len, int.MaxValue);
                this._data = new byte[buffLen];
                
                int offset = 0;
                while (offset < len)
                {
                    offset += ms.Read(this._data, offset, buffLen);
                }

                return offset;
            }
        }

        #endregion

        public override string ToString()
        {
            return this._header.ToString() + ": " + this._data.ToString();
        }
    }
}
