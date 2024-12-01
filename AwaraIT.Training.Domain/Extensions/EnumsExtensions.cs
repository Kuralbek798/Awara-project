using System;
using System.Collections.Generic;
using System.Text;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PosibleDeal;

namespace AwaraIT.Training.Domain.Extensions
{
    /// <summary>
    /// Класс расширений для перечислений.
    /// </summary>
    public static class EnumsExtensions
    {
        /// <summary>
        /// Преобразует значение перечисления PosibleDealStepStatus в целое число.
        /// </summary>
        /// <param name="status">Значение перечисления PosibleDealStepStatus.</param>
        /// <returns>Целое число, представляющее значение перечисления.</returns>
        public static int ToIntValue(this PosibleDealStepStatus status)
        {
            return (int)status;
        }

        /// <summary>
        /// Преобразует значение перечисления InterestStepStatus в целое число.
        /// </summary>
        /// <param name="status">Значение перечисления InterestStepStatus.</param>
        /// <returns>Целое число, представляющее значение перечисления.</returns>
        public static int ToIntValue(this InterestStepStatus status)
        {
            return (int)status;
        }
    }
}
