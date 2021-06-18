using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownSuite.ConversionServer.APISite.Models
{
    public class ItemRequestModel<T>
    {
        public T Data { get; set; }
    }
}
