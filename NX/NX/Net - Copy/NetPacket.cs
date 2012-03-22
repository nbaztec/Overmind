using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NX.Net
{    
    public partial class NetPacket : IBinarySerializer
    {
        public enum CommandType : byte
        {
            None = 0,
            Custom,
            Print,
            Upload,
            Download,
            WriteToFile
        }

        private PacketHeader _header;
        private byte[] _data;
        
        public PacketHeader Header { get { return this._header; } }
        public byte[] Data { get { return this._data; } }

        public NetPacket(int src, int dest, CommandType type, byte[] data)
        {
            this._header = new PacketHeader(src, dest, type);
            this._data = data;
        }

        public NetPacket(int src, int dest, byte[] data)
        {
            this._header = new PacketHeader(src, dest, CommandType.Print);
            this._data = data;
        }


        public override string ToString()
        {
            return this._header.ToString() + ": " + this._data.ToString();
        }

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

        public void Deserialize(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Position = this._header.Deserialize(bytes);
                long len = ms.Length - ms.Position;                
                int buffLen = (int)Math.Min(len, int.MaxValue);
                this._data = new byte[buffLen];
                int offset = 0;
                int _t = 0;
                while ((_t = ms.Read(this._data, offset, buffLen)) > 0)
                {
                    offset += _t;
                }
            }            
        }        
    }
}
