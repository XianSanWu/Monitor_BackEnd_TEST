using System.Reflection;

namespace Utilities.Utilities
{
    public class Reflection
    {
        // 使用反射來獲取模型的有效欄位名稱
        public static HashSet<string> GetValidColumns<T>()
        {
            var validColumns = new HashSet<string>();

            // 獲取類型的所有屬性
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // 這裡簡單處理，將所有公有屬性加入 validColumns，具體邏輯可以根據需求調整
                validColumns.Add(property.Name);
            }

            return validColumns;
        }

        // 使用反射來獲取模型的有效欄位名稱及其對應的值
        public static Dictionary<string, object?> GetValidColumnsWithValues<T>(T model)
        {
            var validColumns = new Dictionary<string, object?>();

            // 獲取類型的所有屬性
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                // 將屬性名稱和其對應的值（可能為 null）加入字典中
                validColumns.Add(property.Name, property.GetValue(model));
            }

            return validColumns;
        }
    }
}
