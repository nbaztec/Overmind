using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NX.Collections;

namespace NX_Overmind
{
    class CaptureRecorder
    {
        public bool Enabled { get; set; }
        public string LogFilename
        {
            get
            {
                return this._file;
            }
        }
        public int BufferLength
        {
            get
            {
                return this._bufferCount;
            }
        }

        private string _file;
        private string _dir;
        private string _tempDir;
        private MemoryStream _ms;        
        private int _bufferCount;

        private int _bufferLimit;        
        private List<string> _includeFileList = null;

        public CaptureRecorder(string filename)
        {
            this.Enabled = false;

            this._file = filename;
            this._dir = Path.GetDirectoryName(filename);
            this._tempDir = Path.Combine(this._dir, Path.GetRandomFileName());
            Directory.CreateDirectory(this._tempDir);
            this._ms = new MemoryStream();

            this._bufferCount = 0;
            this._bufferLimit = 10;
            this._includeFileList = new List<string>();
            
        }

        public void Log(byte[] data)
        {
            if (this.Enabled)
            {                
                string tempFile = Path.Combine(this._tempDir, DateTime.Now.Ticks.ToString());
                this._bufferCount++;
                this._includeFileList.Add(tempFile);

                File.WriteAllBytes(tempFile, data);

                if (this._bufferCount == this._bufferLimit)
                {
                    this.CompressLog(false);
                    this._bufferCount = 0;
                    this._includeFileList = new List<string>();
                    this._tempDir = Path.Combine(this._dir, Path.GetRandomFileName());
                    Directory.CreateDirectory(this._tempDir);
                }
            }
        }

        private void CompressLog(bool wait)
        {
            Compression7z cmp = new Compression7z();
            cmp.OnComplete += new Compression7z.ProcessEventHandler(cmp_OnComplete);
            cmp.CompressFiles(wait, this._file, Compression7z.Type._7z, Compression7z.Compression.Ultra, this._includeFileList.ToArray(), null, null);
        }

        private void cmp_OnComplete(CompressionEventArgs e, bool errorOccured)
        {            
            //foreach (string s in e.IncludeFiles)
            //    File.Delete(s);
            if(e.IncludeFiles.Length != 0)
                Directory.Delete(Path.GetDirectoryName(e.IncludeFiles[0]), true);
        }

        public void Dispose()
        {
            if(this.Enabled)
                this.CompressLog(true);
        }
    }
}
