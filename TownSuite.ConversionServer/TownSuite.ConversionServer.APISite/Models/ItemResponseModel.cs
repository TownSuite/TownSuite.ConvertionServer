using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TownSuite.ConversionServer.Common.Models.Errors;

namespace TownSuite.ConversionServer.APISite.Models
{
    public class ItemResponseModel<T>
    {
        public T Data { get; set; }
        public ResponseErrorModel Error {get;set;}
    }
}
