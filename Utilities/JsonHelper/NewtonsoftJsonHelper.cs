using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Utilities.JsonHelper

{
    /// <summary>
    /// 不轉換編碼
    /// </summary>

    public static class NewtonsoftJsonHelper
    {
        private static readonly JsonSerializerSettings _settings = new()
        {
            StringEscapeHandling = StringEscapeHandling.Default,
            Formatting = Formatting.Indented,
            Converters = [new StringEnumConverter()]
        };

        public static string SerializeObject<T>(T obj) => JsonConvert.SerializeObject(obj, _settings)?.Replace("\r", "")?.Replace("\n", "") ?? string.Empty;

        public static T DeserializeObject<T>(string json)
        {
            var result = JsonConvert.DeserializeObject<T>(json, _settings);
            return result == null ? throw new JsonSerializationException("反序列化失敗：傳入的 JSON 無效 或 不符模型") : result;
        }

    }

}