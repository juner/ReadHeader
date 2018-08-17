using System;
using System.Collections.Generic;
using System.Text;

namespace DiskHeader
{
    public interface IDiskHeaders
    {
        MBR.Header Mbr { get; }
    }
}
