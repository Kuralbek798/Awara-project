using System;
using System.Collections.Generic;
using System.Text;
using static AwaraIT.Training.Domain.Models.Crm.Entities.Interest;
using static AwaraIT.Training.Domain.Models.Crm.Entities.PosibleDeal;

namespace AwaraIT.Training.Domain.Extensions
{
    public static class EnumsExtensions
    {
        public static int ToIntValue(this PosibleDealStepStatus status)
        {
            return (int)status;
        }

        public static int ToIntValue(this InterestStepStatus status)
        {
            return (int)status;
        }
    }
}
