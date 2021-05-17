using System;
using System.Collections.Generic;
using System.Text;
using TownSuite.ConversionServer.Interfaces.Common.Bytes;

namespace TownSuite.ConversionServer.Common.Bytes
{
    public class ByteCountFactory : IByteCountFactory
    {
        public IByteCount FromGigabytes(decimal gigabytes)
        {
            return new ByteCount()
            {
                MaxGigabytes = gigabytes
            };
        }
    }
}
