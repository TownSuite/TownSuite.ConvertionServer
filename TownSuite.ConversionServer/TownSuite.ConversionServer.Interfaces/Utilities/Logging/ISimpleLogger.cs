using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TownSuite.ConversionServer.Interfaces.Utilities.Logging
{
    public interface ISimpleLogger
    {
        public const int DEFAULT_ERROR_LEVEL = 5;

        Task LogError(Exception ex);
        Task LogError(Exception ex, int level);
        Task LogError(Exception ex, string messageOverride, int level = DEFAULT_ERROR_LEVEL);
        Task LogError(string message, string details, int level = DEFAULT_ERROR_LEVEL);
    }
}
