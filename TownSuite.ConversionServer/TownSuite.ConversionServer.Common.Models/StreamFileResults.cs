using System.IO;

namespace TownSuite.ConversionServer.Common.Models
{
    public class StreamFileResults
    {
        public Stream File { get; }
        public string MediaType { get; }

        public StreamFileResults(Stream file, string mediaType)
        {
            File = file;
            MediaType = mediaType;
        }
    }
}
