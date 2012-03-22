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
        /// Denotes the header of a NetPacket
        /// </summary>
        public class PacketHeader : IBinarySerializer
        {
            private int _source;
            private int _destination;
            private PacketType _type;
            private long _timestamp;

            public int Source { get { return this._source; } }
            public int Destination { get { return this._destination; } }
            public PacketType Type { get { return this._type; } }
            public long Timestamp { get { return this._timestamp; } }

            #region Constructors

            /// <summary>
            /// Constructor
            /// </summary>
            public PacketHeader()
            {
                this._timestamp = this._source = this._destination = -1;
                this._type = PacketType.None;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="src">Source Id</param>
            /// <param name="dest">Destination Id</param>
            /// <param name="type">Packet type</param>
            public PacketHeader(int src, int dest, PacketType type)
            {
                this._source = src;
                this._destination = dest;
                this._type = type;
                this._timestamp = DateTime.UtcNow.ToBinary();
            }

            #endregion

            #region Interface Methods

            /// <summary>
            /// Serializes PacketHeader object
            /// </summary>
            /// <returns>Data bytes</returns>
            public byte[] Serialize()
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] data = BitConverter.GetBytes(this._source);
                    ms.Write(data, 0, data.Length);
                    
                    data = BitConverter.GetBytes(this._destination);
                    ms.Write(data, 0, data.Length);

                    data = BitConverter.GetBytes(this._timestamp);
                    ms.Write(data, 0, data.Length);

                    ms.WriteByte((byte)this._type);

                    return ms.ToArray();
                }
            }

            /// <summary>
            /// Deserializes PacketHeader object
            /// </summary>
            /// <param name="bytes">Data bytes</param>
            /// <returns>Length of bytes read</returns>
            public long Deserialize(byte[] bytes)
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    byte[] data = new byte[sizeof(int)];
                    ms.Read(data, 0, data.Length);
                    this._source = BitConverter.ToInt32(data,0);
                    
                    ms.Read(data, 0, data.Length);
                    this._destination = BitConverter.ToInt32(data,0);
                    
                    data = new byte[sizeof(long)];
                    ms.Read(data, 0, data.Length);
                    this._timestamp = BitConverter.ToInt64(data, 0);
                    
                    this._type = (PacketType)ms.ReadByte();

                    return ms.Position;
                }
            }

            #endregion
        }            
    }
}