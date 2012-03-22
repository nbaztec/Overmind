using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace NX.Collections
{
    /// <summary>
    /// Represents the event arguments
    /// </summary>
    public class CompressionEventArgs
    {
        private Compression7z.Action _operation;
        private string _trgt;
        private Compression7z.Type _type;
        private Compression7z.Compression _cmp;
        private string _pwd;
        private string[] _include;
        private string[] _exclude;

        public Compression7z.Action Operation { get { return this._operation; } }
        public string TargetFile { get { return this._trgt; } }
        public Compression7z.Type FileType { get { return this._type; } }
        public Compression7z.Compression Compression { get { return this._cmp; } }
        public string Password { get { return this._pwd; } }
        public string[] IncludeFiles { get { return this._include; } }
        public string[] ExcludeFiles { get { return this._exclude; } }

        public bool HasPassword { get { return (this._pwd != null) && (this._pwd.Trim() != ""); } }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="operation">Type of operation</param>
        /// <param name="targetName">Filename of target file</param>
        /// <param name="type">Type of archive</param>
        /// <param name="compressionLevel">Compression level</param>
        /// <param name="include">List of files to be included</param>
        /// <param name="exclude">List of files to be excluded</param>
        /// <param name="password">Optional password for the archive.</param>
        public CompressionEventArgs(Compression7z.Action operation, string targetName, Compression7z.Type type, Compression7z.Compression compressionLevel, string[] include, string[] exclude, string password)
        {
            this._operation = operation;
            this._trgt = targetName;
            this._type = type;
            this._cmp = compressionLevel;
            this._pwd = password;
            this._include = include;
            this._exclude = exclude;
        }
    }

    /// <summary>
    /// Manages the 7z operations
    /// </summary>
    public class Compression7z
    {
        public enum Action
        {
            Compress,
            Extract
        }

        public enum Compression : uint
        {
            CopyOnly = 0,
            Fast    = 3,
            Normal  = 5,
            Maximum = 7,
            Ultra   = 9  
        }

        public enum Type : uint
        {
            _7z,
            bzip2,
            gzip,
            iso,
            tar,
            udf,
            zip
        }

        private string _path;
        //private bool _silent;

        /// <summary>
        /// Internal delegate for operation complete
        /// </summary>
        /// <param name="sender"></param>
        private delegate void CompressionCompleteHandler(_7zComp sender);

        /// <summary>
        /// External delegate to raise events
        /// </summary>
        /// <param name="e">Event options</param>
        /// <param name="errorOccured">Whether error occurred in operation</param>
        public delegate void ProcessEventHandler(CompressionEventArgs e, bool errorOccured);
        public event ProcessEventHandler OnComplete = null;                      

        /// <summary>
        /// Constructor
        /// </summary>
        public Compression7z()
        {
            this._path = "7z.exe";
            //this._silent = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path7z">Path to 7z executable (7z.exe)</param>
        public Compression7z(string path7z)            
        {
            this._path = path7z;
        }
        
        /*        
         * Silent is not implemented/required
         * 
         * 
        public Compression7z(bool silent)
            : this()
        {
            this._silent = silent;
        }

        public Compression7z(string path, bool silent)
            : this()
        {
            this._path = path;
            this._silent = silent;
        }
         * 
         * 
         * 
         */
        
        /// <summary>
        /// Checks if the 7z executable is present in the path specified
        /// </summary>
        /// <returns>True if 7z execuatble is present</returns>
        public bool Is7zPresent()
        {
            return File.Exists(this._path);
        }

        /// <summary>
        /// Compresses the files specified
        /// </summary>
        /// <param name="wait">Wait for the process to finish</param>
        /// <param name="targetName">Target name of the archive</param>
        /// <param name="type">Type of archive</param>
        /// <param name="compressionLevel">Compression level</param>
        /// <param name="include">List of files to be included</param>
        /// <param name="exclude">List of files to be excluded</param>
        /// <param name="password">Optional password for the archive.</param>        
        public void CompressFiles(bool wait, string targetName, Type type, Compression compressionLevel, string[] include, string[] exclude, string password)
        {
            if (!this.Is7zPresent())            
                throw new FileNotFoundException("The 7z executable was not found.");
            
            // Create new Thread of compressor
            new _7zComp(wait, this._path, new CompressionEventArgs(Action.Compress, targetName, type, compressionLevel, include, exclude, password), new CompressionCompleteHandler(this.ActionComplete));
        }

        /// <summary>
        /// Extracts the archive to a destination directory
        /// </summary>
        /// <param name="wait">Wait for the process to finish</param>
        /// <param name="targetName">Target name of the archive</param>
        /// <param name="outputDir">Output directory for extraction</param>
        /// <param name="type">Type of archive</param>
        /// <param name="compressionLevel">Compression level</param>        
        /// <param name="password">Optional password for the archive.</param>         
        public void ExtractFile(bool wait, string targetName, string outputDir, Type type, Compression compressionLevel, string password)
        {
            if (!this.Is7zPresent())
                throw new FileNotFoundException("The 7z executable was not found.");
            if (outputDir == null)
                throw new ArgumentNullException("Output directory cannot be null.");
            // Create new Thread of compressor
            new _7zComp(wait, this._path, new CompressionEventArgs(Action.Extract, targetName, type, compressionLevel, new string[]{outputDir}, null, password), new CompressionCompleteHandler(this.ActionComplete));
        }

        /// <summary>
        /// Extracts the archive to the default directory containing it
        /// </summary>
        /// <param name="wait">Wait for the process to finish</param>
        /// <param name="targetName">Target name of the archive</param>        
        /// <param name="type">Type of archive</param>
        /// <param name="compressionLevel">Compression level</param>        
        /// <param name="password">Optional password for the archive.</param>         
        public void ExtractFile(bool wait, string targetName, Type type, Compression compressionLevel, string password)
        {
            this.ExtractFile(wait, targetName, Directory.GetParent(targetName).FullName, type, compressionLevel, password);
        }

        /// <summary>
        /// Callback when operation is completed
        /// </summary>
        /// <param name="sender">Object who raised the event</param>
        private void ActionComplete(_7zComp sender)
        {
            if (this.OnComplete != null)
                this.OnComplete(sender.Arguments, sender.ErrorOccured);
        }

        /// <summary>
        /// Internal class to manage the basic 7z operations
        /// </summary>
        private class _7zComp
        {
            private bool _errorOccured;
            private readonly CompressionEventArgs _args;
            private readonly CompressionCompleteHandler _callback;
            private readonly string _path;           

            public bool ErrorOccured { get { return this._errorOccured; } }
            public CompressionEventArgs Arguments { get { return this._args; } }            

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="wait">Wait for the process to finish</param>
            /// <param name="path">Path to 7z executable</param>
            /// <param name="args">Arguments to the 7z executable</param>
            /// <param name="callback">Callback method for completion</param>
            public _7zComp(bool wait, string path, CompressionEventArgs args, CompressionCompleteHandler callback)
            {
                this._errorOccured = false;
                this._callback = callback;
                this._args = args;
                this._path = path;

                Thread t = null;
                if (args.Operation == Action.Compress)
                    t = new Thread(new ParameterizedThreadStart(this._Compress));
                else if (args.Operation == Action.Extract)
                    t = new Thread(new ParameterizedThreadStart(this._Extract));
                t.Name = "_7zThread";
                t.Start(args);
                
                if (wait)
                    t.Join();
            }

            /// <summary>
            /// Executes the 7z executable
            /// </summary>
            /// <param name="args">Arguments for the process</param>
            private void _Launch(string args)
            {
                ProcessStartInfo psInfo = new ProcessStartInfo();
                psInfo.FileName = this._path;
                psInfo.Verb = "Open";
                psInfo.UseShellExecute = false;
                //psInfo.RedirectStandardOutput = true;
                psInfo.WindowStyle = ProcessWindowStyle.Normal;
                psInfo.CreateNoWindow = true;
                psInfo.WorkingDirectory = "";

                psInfo.Arguments = args;

                this._errorOccured = false;
                Process ps = Process.Start(psInfo);
                ps.WaitForExit();
                this._errorOccured = ps.ExitCode != 0;
                if (this._callback != null)
                    this._callback(this);
            }            
            
            /// <summary>
            /// Prepares and launches the 7z executable for compression
            /// </summary>
            /// <param name="args">CompressionEventArgs object representing the arguments</param>
            private void _Compress(object args)
            {
                CompressionEventArgs e = args as CompressionEventArgs;

                string excl = "";
                string incl = "";
                string pwd = "";

                string t;
                // Compute ignore files
                if (e.ExcludeFiles != null)
                {
                    foreach (string s in e.ExcludeFiles)                    
                        if ((t = s.Trim()) != String.Empty)
                            excl += string.Format("-xr!\"{0}\" ", t);
                    
                }
                // Compute include files                
                foreach (string s in e.IncludeFiles)
                {
                    if ((t = s.Trim()) != String.Empty)
                        incl += string.Format("\"{0}\" ", t);
                }

                if (e.Password != null && e.Password.Trim() != "")
                    pwd = "-p" + e.Password;

                this._Launch(string.Format("u -t{1} \"{0}.{1}\" {3} -mx{2} {4} {5}", e.TargetFile, this.FromType(e.FileType), this.FromCompression(e.Compression), incl.TrimEnd(), excl.TrimEnd(), pwd));                
            }

            /// <summary>
            /// Prepares and launches the 7z executable for extraction
            /// </summary>
            /// <param name="args">CompressionEventArgs object representing the arguments</param>
            /// <remarks>args.IncludeFiles[0] is used to contain the output directory</remarks>
            private void _Extract(object args)
            {
                CompressionEventArgs e = args as CompressionEventArgs;
                // e.IncludeFiles[0] is the output directory
                string pwd = (e.Password != null && e.Password.Trim() != "") ? ("-p" + e.Password) : "";
                this._Launch(string.Format("x -t{1} \"{0}\" -mx{2} {3} -o\"{4}\"", e.TargetFile, this.FromType(e.FileType), this.FromCompression(e.Compression), pwd, e.IncludeFiles[0]));                
            }

            /// <summary>
            /// Makes extensions form the archive type enumeration
            /// </summary>
            /// <param name="t">Archive Type enum</param>
            /// <returns>string representing the extension</returns>
            private string FromType(Type t)
            {
                return t.ToString().TrimStart('_');
            }

            /// <summary>
            /// Makes extensions form the archive compression level enumeration
            /// </summary>
            /// <param name="c">Archive Compression level enum</param>
            /// <returns>string representing the compression level (number)</returns>
            private string FromCompression(Compression c)
            {
                return ((uint)c).ToString();
            }
        }       
    }    
}
