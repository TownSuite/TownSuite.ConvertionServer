using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.Interfaces.Common.Errors
{
    public interface IResponseErrorModelFactory<TErrorModel>
    {
        TErrorModel Create(string message, string details);
        TErrorModel Create(Exception ex);
        TErrorModel Create(Exception ex, string messageOverride);
    }
}
