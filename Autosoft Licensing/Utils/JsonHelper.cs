using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Autosoft_Licensing.Utils
{
    public static class JsonHelper
    {
        // Canonicalize JSON by sorting properties recursively; stable UTF-8 text
        public static string Canonicalize(string json)
        {
            var token = JToken.Parse(json);
            var canon = SortToken(token);
            return canon.ToString(Formatting.None);
        }

        public static string RemoveProperty(string json, string propertyName)
        {
            var token = JToken.Parse(json);
            RemovePropertyRecursive(token, propertyName);
            return token.ToString(Formatting.None);
        }

        private static JToken SortToken(JToken token)
        {
            if (token is JObject obj)
            {
                var props = obj.Properties().OrderBy(p => p.Name)
                    .Select(p => new JProperty(p.Name, SortToken(p.Value)));
                return new JObject(props);
            }
            if (token is JArray arr)
            {
                return new JArray(arr.Select(SortToken));
            }
            return token;
        }

        private static void RemovePropertyRecursive(JToken token, string name)
        {
            if (token is JObject obj)
            {
                obj.Property(name)?.Remove();
                foreach (var child in obj.Properties())
                    RemovePropertyRecursive(child.Value, name);
            }
            else if (token is JArray arr)
            {
                foreach (var item in arr) RemovePropertyRecursive(item, name);
            }
        }
    }
}