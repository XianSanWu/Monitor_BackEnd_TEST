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
    }
}
