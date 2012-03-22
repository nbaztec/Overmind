using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;

namespace NX_Overmind
{    
    public partial class CapturePacket : IStreamCodec
    {
        private Stream _screen;
        private Stream _log;


        #region Properties

        public Stream ScreenShot { get { return this._screen; } }
        public Stream Log { get { return this._log; } }        
        /// <summary>
        /// Parent directory for storing files
        /// </summary>
        public string ParentDirectory { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public CapturePacket()
        {
            this.ParentDirectory = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="screenShot">Screenshot</param>
        /// <param name="log">Event log stream</param>
        public CapturePacket(Stream screenShot, Stream log)
            : this()
        {
            this._screen = screenShot;
            this._log = log;
        }

        #endregion

        /// <summary>
        /// Gets a filename
        /// </summary>
        /// <param name="filename">Optional filename. Timestamp is used if null</param>
        /// <returns>Filename</returns>
        protected string GetFileName(object filename)
        {
            return Path.Combine(this.ParentDirectory, (filename == null) ? DateTime.UtcNow.Ticks.ToString() : filename as string);
        }

        /// <summary>
        /// Saves the screenshot to a file
        /// </summary>
        /// <param name="file">Filename</param>
        public void SaveScreen(string file)
        {
            Image im = Image.FromStream(this._screen);
            im.Save(file);
        }

        #region IStreamCodec Members

        public MemoryStream Encode()
        {
            MemoryStream ms = new MemoryStream();
            ms.Position = 0;
            MemoryStream ss = (MemoryStream)this._screen;
            MemoryStream ls = (MemoryStream)this._log;

            if (ss != null)
            {
                this.WriteHeader(ms, (int)ss.Length);
                ss.WriteTo(ms);
            }
            else
                this.WriteHeader(ms, 0);

            if (ls != null)
            {
                this.WriteHeader(ms, (int)ls.Length);
                ls.WriteTo(ms);
            }
            else
                this.WriteHeader(ms, 0);

            return ms;
        }

        public void Decode(MemoryStream ms)
        {
            int len = 0;
            byte[] data = null;

            len = this.ReadHeader(ms);
            data = new byte[len];
            ms.Read(data, 0, len);
            this._screen = new MemoryStream(data);

            len = this.ReadHeader(ms);
            data = new byte[len];
            ms.Read(data, 0, len);
            this._log = new MemoryStream(data);
        }

        #endregion

        protected int ReadHeader(MemoryStream ms)
        {
            byte[] data = new byte[4];
            ms.Read(data, 0, 4);
            return BitConverter.ToInt32(data, 0);
        }

        protected void WriteHeader(MemoryStream ms, int len)
        {
            ms.Write(BitConverter.GetBytes(len), 0, 4);
        }

        public string WriteEncoded(string file)
        {
            file = this.GetFileName(file);
            using (MemoryStream ms = this.Encode())
            {
                using (FileStream fs = new FileStream(file, FileMode.Create))
                {
                    fs.Write(ms.GetBuffer(), 0, (int)ms.Position);
                }
            }

            return file;
        }

        public void ReadDecoded(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                using (MemoryStream ms = new MemoryStream(data))
                {
                    this.Decode(ms);
                }
            }
        }       
    }
}
