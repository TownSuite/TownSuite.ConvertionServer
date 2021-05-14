using System;
using System.Collections.Generic;
using System.Text;

namespace TownSuite.ConversionServer.Interfaces.Utilities.Serializers
{
    public interface IGeneralSerializer
    {
        public string Serialize(object o);
        public T Deserialize<T>(string json);
    }
}
