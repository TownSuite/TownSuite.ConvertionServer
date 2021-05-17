using System;
using System.Collections.Generic;
using System.Text;
using TownSuite.ConversionServer.Interfaces.Common.Bytes;

namespace TownSuite.ConversionServer.Common.Bytes
{
    public class ByteCount: IByteCount
    {
        public const int BINARY_BASE = 1024;
        public const int DEFAULT_BASE = 1000;
        public int Base;

        public ByteCount()
        {
            Base = DEFAULT_BASE;
        }

        public long Bytes
        {
            get;
            set;
        }
        public decimal Kilobytes
        {
            get => Bytes / 1000M;
            set => Bytes = (long)Math.Floor(value * 1000M);
        }
        public decimal Megabytes
        {
            get => Kilobytes / 1000M;
            set => Kilobytes = value * 1000M;
        }
        public decimal MaxGigabytes
        {
            get => Megabytes / 1000M;
            set => Kilobytes = value * 1000M;
        }
    }
}
