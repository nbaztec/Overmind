using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NX.Net
{
    public partial class NetPacket : IBinarySerializer
    {
        public class PacketHeader : IBinarySerializer
        {
            public int _source;
            public int _destination;
            public CommandType _type;
            public long _timestamp;

            public int Source { get { return this._source; } }
            public int Destination { get { return this._destination; } }
            public CommandType Type { get { return this._type; } }
            public long Timestamp { get { return this._timestamp; } }
           
            public PacketHeader(int src, int dest, CommandType type)
            {
                this._source = src;
                this._destination = dest;
                this._type = type;
                this._timestamp = DateTime.UtcNow.ToBinary();
            }

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

            public int Deserialize(byte[] bytes)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] data = new byte[sizeof(int)];
                    ms.Read(data, 0, data.Length);
                    this._source = BitConverter.ToInt32(data,0);
                    
                    ms.Read(data, 0, data.Length);
                    this._destination = BitConverter.ToInt32(data,0);
                    
                    data = new byte[sizeof(long)];
                    ms.Read(data, 0, data.Length);
                    this._timestamp = BitConverter.ToInt64(data, 0);
                    
                    this._type = (CommandType)ms.ReadByte();

                    return ms.Position;
                }
            }
        }

    }
}