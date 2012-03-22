using System;
using System.IO;

namespace NX.Collections
{
    /// <summary>
    /// Disposable directory, removes itself on Dispose()
    /// </summary>
    public class DisposableDirectory : IDisposable
    {
        protected string _path;
        public string DirectoryPath { get { return this._path; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePath">Base path for the directory</param>
        /// <param name="dirName">Directory name</param>
        public DisposableDirectory(string basePath, string dirName)
        {
            this._path = Path.Combine(basePath, dirName);
            if (!Directory.Exists(this._path))
                Directory.CreateDirectory(this._path);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="basePath">Base path for the directory</param>        
        public DisposableDirectory(string basePath)
            : this(basePath, "nx-"+Path.GetRandomFileName())
            //:this(basePath, "nx-folder")//Path.GetRandomFileName())
        {            
        }

        /// <summary>
        /// Constructor, initializes a random directory in TEMP directory
        /// </summary>                
        public DisposableDirectory()
            : this(Path.GetTempPath())
        {            
        }

        /// <summary>
        /// Disposes the directory
        /// </summary>
        public void Dispose()
        {
            System.Diagnostics.Trace.WriteLine("Disposing: " + this._path);
            Directory.Delete(this._path, true);
            //this.DeleteFileSystemInfo(new DirectoryInfo(this._path));
        }        

        /// <summary>
        /// Recursively deletes the directory
        /// </summary>
        /// <param name="fsi">Directory to delete</param>
        protected void DeleteFileSystemInfo(FileSystemInfo fsi)
        {            
            fsi.Attributes = FileAttributes.Normal;
            DirectoryInfo di = fsi as DirectoryInfo;

            if (di != null)            
                foreach (FileSystemInfo dirInfo in di.GetFileSystemInfos())                
                    this.DeleteFileSystemInfo(dirInfo);                            

            fsi.Delete();
        }
        
        public override string ToString()
        {
            return string.Format("'{0}'", this._path);
        }
    }
}
