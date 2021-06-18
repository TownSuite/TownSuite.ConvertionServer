using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.Interfaces.Common.Bytes
{
    public interface IByteCount
    {
        long Bytes { get; }
        decimal Kilobytes { get; }
        decimal Megabytes { get; }
        decimal MaxGigabytes { get; }
    }
}
