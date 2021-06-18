using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TownSuite.ConversionServer.Interfaces.Utilities.Converters
{
    public interface IPdfToImageBytesConverter
    {
        Task<IEnumerable<byte[]>> Convert(byte[] pdf, CancellationToken cancellationToken = default);
    }
}
