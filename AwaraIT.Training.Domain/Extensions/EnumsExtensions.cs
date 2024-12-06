using System;
using System.Collections.Generic;
using System.Text;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PossibleDeal;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PriceList;

namespace AwaraIT.Training.Domain.Extensions
{
    /// <summary>
    /// Класс расширений для перечислений.
    /// </summary>
    public static class EnumsExtensions
    {
        /// <summary>
        /// Преобразует значение перечисления  в целое число.
        /// </summary>
        /// <param name="status">Значение перечисления .</param>
        /// <returns>Целое число, представляющее значение перечисления.</returns>
        public static int ToIntValue<T>(this T status) where T : Enum
        {
            return Convert.ToInt32(status);
        }
    }
}
