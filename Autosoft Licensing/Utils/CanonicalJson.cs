using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Autosoft_Licensing.Utils
{
    /// <summary>
    /// Public canonical JSON API.
    /// Produces deterministic canonical JSON bytes that match the generator pipeline.
    /// </summary>
    public static class CanonicalJson
    {
        private static readonly JsonSerializerSettings CanonicalSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Include,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Converters = new[] { new StringEnumConverter() }
        };

        /// <summary>
        /// Serialize object to a canonical JSON string (compact, sorted keys, ISO dates).
        /// </summary>
        public static string SerializeCanonical(object obj)
        {
            if (obj == null) return "{}";

            try
            {
                // 1. Create a JToken from the object using settings that ensure
                //    DateTime values are in the correct ISO 8601 format and enums are strings.
                var token = JToken.FromObject(obj, JsonSerializer.Create(CanonicalSettings));

                // 2. Remove the checksum property from the JToken itself.
                if (token is JObject jObject)
                {
                    jObject.Property("ChecksumSHA256")?.Remove();
                }

                // 3. Recursively sort the properties of the JToken.
                var sortedToken = SortToken(token);

                // 4. Convert the final, sorted token to a compact string.
                return sortedToken.ToString(Formatting.None);
            }
            catch
            {
                // Defensive: do not throw from a helper.
                return "{}";
            }
        }

        /// <summary>
        /// Serialize object to canonical JSON UTF8 bytes (no BOM).
        /// </summary>
        public static byte[] SerializeCanonicalToUtf8Bytes(object obj)
        {
            var s = SerializeCanonical(obj);
            return Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// Recursively sorts a JToken. JObject properties are sorted by name, JArrays are processed recursively.
        /// </summary>
        private static JToken SortToken(JToken token)
        {
            if (token is JObject obj)
            {
                var props = obj.Properties().OrderBy(p => p.Name, StringComparer.Ordinal);
                var newObj = new JObject();
                foreach (var prop in props)
                {
                    newObj.Add(prop.Name, SortToken(prop.Value));
                }
                return newObj;
            }

            if (token is JArray arr)
            {
                return new JArray(arr.Select(SortToken));
            }

            return token;
        }
    }
}
