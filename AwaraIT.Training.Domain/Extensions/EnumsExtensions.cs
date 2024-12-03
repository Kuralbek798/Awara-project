using System;
using System.Collections.Generic;
using System.Text;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PossibleDeal;

namespace AwaraIT.Training.Domain.Extensions
{
    /// <summary>
    /// Класс расширений для перечислений.
    /// </summary>
    public static class EnumsExtensions
    {
        /// <summary>
        /// Преобразует значение перечисления PossibleDealStepStatus в целое число.
        /// </summary>
        /// <param name="status">Значение перечисления PossibleDealStepStatus.</param>
        /// <returns>Целое число, представляющее значение перечисления.</returns>
        public static int ToIntValue(this PossibleDealStepStatus status)
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
