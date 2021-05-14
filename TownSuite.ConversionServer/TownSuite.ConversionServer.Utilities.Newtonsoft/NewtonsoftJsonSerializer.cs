using System;
using Newtonsoft.Json;
using TownSuite.ConversionServer.Interfaces.Utilities.Serializers;

namespace TownSuite.ConversionServer.Utilities.Newtonsoft
{
    public class NewtonsoftJsonSerializer: IJsonSerializer
    {
        public string Serialize(object o)
        {
            return JsonConvert.SerializeObject(o);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
