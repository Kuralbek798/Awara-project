using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwaraIT.Kuralbek.Plugins.Hellpers
{
    /// <summary>
    /// Класс <c>DataForLogs</c> содержит методы для форматирования данных для журналов.
    /// </summary>
    public static class DataForLogs
    {
        /// <summary>
        /// Преобразует список GUID-ов в строку, разделенную запятыми.
        /// </summary>
        /// <param name="guids">Список GUID-ов.</param>
        /// <returns>Строка, содержащая GUID-ы, разделенные запятыми.</returns>
        public static string GetGuidsString(List<Guid> guids)
        {
            return string.Join(", ", guids.Select(g => g.ToString()));
        }

        /// <summary>
        /// Преобразует словарь данных в строку, где каждая пара ключ-значение представлена в формате "key: {ключ} : value: {значение}".
        /// </summary>
        /// <param name="data">Словарь данных.</param>
        /// <returns>Строка, содержащая пары ключ-значение, разделенные запятыми.</returns>
        public static string GetDataStringFromDictionary(Dictionary<Guid, int> data)
        {
            return string.Join(", ", data.Select(entry => $"key: {entry.Key} : value: {entry.Value}"));
        }
    }
}






