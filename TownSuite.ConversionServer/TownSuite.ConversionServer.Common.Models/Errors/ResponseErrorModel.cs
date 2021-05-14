using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownSuite.ConversionServer.Common.Models.Errors
{
    public class ResponseErrorModel
    {
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
