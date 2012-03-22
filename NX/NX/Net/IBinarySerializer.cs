using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NX
{
    /// <summary>
    /// Interface for serilization and deserialization
    /// </summary>
    interface IBinarySerializer
    {
        byte[] Serialize();
        long Deserialize(byte[] bytes);
    }
}
