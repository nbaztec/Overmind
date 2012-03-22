using System;
using System.IO;

namespace NX_Overmind
{
    /// <summary>
    /// Interface for encoding and decoding streams
    /// </summary>
    interface IStreamCodec
    {
        MemoryStream Encode();
        void Decode(MemoryStream ms);
    }
}
