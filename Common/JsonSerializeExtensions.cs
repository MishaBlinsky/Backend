using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class JsonSerializeExtensions
    {
        private static readonly JsonSerializerSettings Formatter = new()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            },
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            }
        };
        public static string ToJson<T>(this T obj) => JsonConvert.SerializeObject(obj, Formatter);
        public static T FromJson<T>(this string json) => JsonConvert.DeserializeObject<T>(json, Formatter)!;
    }
}