using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.Interfaces.Common.Bytes
{
    public interface IByteCountFactory
    {
        IByteCount FromGigabytes(decimal gigabytes);
    }
}
