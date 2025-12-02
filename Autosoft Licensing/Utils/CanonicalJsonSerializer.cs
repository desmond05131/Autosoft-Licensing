using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Autosoft_Licensing.Utils
{
    /// <summary>
    /// Produces a deterministic, canonical JSON string from an object.
    /// - Sorts object properties alphabetically.
    /// - Omits null-valued properties.
    /// - Serializes enums as strings.
    /// - Serializes DateTime as UTC ISO 8601 strings.
    /// - Produces compact JSON (no indentation).
    /// </summary>
    public static class CanonicalJsonSerializer
    {
        private static readonly JsonSerializerSettings CanonicalSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore, // Omit null values
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Converters = new[] { new StringEnumConverter() }
        };

        /// <summary>
        /// Serializes an object to a canonical JSON string.
        /// </summary>
        public static string Serialize(object obj)
        {
            if (obj == null) return "{}";

            try
            {
                // 1. Create a JToken from the object using the correct settings.
                var token = JToken.FromObject(obj, JsonSerializer.Create(CanonicalSettings));

                // 2. Recursively sort the properties of the JToken.
                var sortedToken = SortToken(token);

                // 3. Convert the final, sorted token to a compact string.
                return sortedToken.ToString(Formatting.None);
            }
            catch
            {
                // Defensive: do not throw from a helper.
                return "{}";
            }
        }

        /// <summary>
        /// Serializes an object to canonical JSON UTF8 bytes (no BOM).
        /// </summary>
        public static byte[] SerializeToUtf8Bytes(object obj)
        {
            var s = Serialize(obj);
            return Encoding.UTF8.GetBytes(s);
        }

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
