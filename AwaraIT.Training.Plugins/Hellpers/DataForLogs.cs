using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwaraIT.Kuralbek.Plugins.Hellpers
{
    public static class DataForLogs
    {
        public static string GetGuidsString(List<Guid> guids)
        {
            return string.Join(", ", guids.Select(g => g.ToString()));
        }

        public static string GetDataStringFromDictionary(Dictionary<Guid, int> data)
        {
          return string.Join(", ", data.Select(entry => $"key: {entry.Key} : value: {entry.Value}"));
        }
    }
}
