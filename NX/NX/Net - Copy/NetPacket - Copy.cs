using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NX.Net
{
    [Serializable]
    public class NetPacket : BinarySerializer
    {
        [Serializable]
        public class Entity
        {
            public readonly String IP;
            public readonly String Name;

            public Entity(Entity e)
            {
                this.IP = e.IP;
                this.Name = e.Name;
            }

            public Entity(String ip, String name)
            {
                this.IP = ip;
                this.Name = name;
            }

            public override string ToString()
            {
                return this.Name + "[" + this.IP + "]";
            }
        }

        public enum CommandType
        {
            None = 0,
            Custom,
            Print,
            Upload,
            Download,
            WriteToFile
        }

        public CommandType CommType { get; set; }
        public object Arguments;
        public readonly DateTime Timestamp;
        public readonly Entity From;
        public readonly Entity To;

        public NetPacket(CommandType ct, object Args, Entity From, Entity To)
        {
            this.CommType = ct;
            this.Arguments = Args;
            this.Timestamp = DateTime.Now;
            this.From = new Entity(From);
            this.To = new Entity(To);
        }

        public NetPacket(CommandType ct, Entity From, Entity To)
        {
            this.CommType = ct;
            this.Arguments = null;
            this.Timestamp = DateTime.Now;
            this.From = new Entity(From);
            this.To = new Entity(To);
        }

        public NetPacket(object Args, Entity From, Entity To)
        {
            this.CommType = CommandType.None;
            this.Arguments = Args;
            this.Timestamp = DateTime.Now;
            this.From = new Entity(From);
            this.To = new Entity(To);
        }

        public override string ToString()
        {
            return this.CommType.ToString() + ": " + this.Arguments.ToString();
        }

        #region BinarySerializer Members

        public byte[] Serialize()
        {
            byte[] bytes;
            return bytes;
            //throw new NotImplementedException();
        }

        public void Deserialize(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
