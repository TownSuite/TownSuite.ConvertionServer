using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.Interfaces.Utilities.Logging
{
    public interface IModelLogger<TErrorModel>
    {
        System.Threading.Tasks.Task PostAsync(TErrorModel log, System.Threading.CancellationToken cancellationToken = default);
    }
}
