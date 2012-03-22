using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NX.Net
{
    interface IBinarySerializer
    {
        byte[] Serialize();
        void Deserialize(byte[] bytes);
    }
}
