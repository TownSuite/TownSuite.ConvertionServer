using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Interfaces.Common.Bytes;

namespace TownSuite.ConversionServer.Interfaces.Common.Validation
{
    public interface IUploadedStreamHandler
    {
        Task CopyToAsync(Stream streamToCopyTo, IByteCount maxBytes, CancellationToken token);
    }
}