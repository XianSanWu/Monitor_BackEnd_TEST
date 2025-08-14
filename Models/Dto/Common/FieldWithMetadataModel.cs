using System.Text.Json;
using YamlDotNet.Core.Tokens;

namespace Models.Dto.Common
{
    public class FieldWithMetadataModel
    {
        /// <summary> 欄位名稱 </summary>
        public string? Key { get; set; } = "";

        /// <summary> 數學符號 </summary>
        public string? MathSymbol { get; set; } = ""; //MathSymbolEnum

        /// <summary> 值 </summary>  //因前端傳入會變成 JsonElement，所以這邊要處理一下
        private object? _value;
        public object? Value
        {
            get
            {
                if (_value is JsonElement je)
                {
                    return je.ValueKind switch
                    {
                        JsonValueKind.String => je.GetString(),
                        JsonValueKind.Number => je.GetDecimal(),
                        JsonValueKind.True or JsonValueKind.False => je.GetBoolean(),
                        _ => je.ToString()
                    };
                }
                return _value;
            }
            set => _value = value;
        }
    }

}
