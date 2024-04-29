using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownSuite.ConversionServer.Common.Models.Services
{
    public class ItemRequestModel<T>
    {
        public T Data { get; set; }
    }
}
