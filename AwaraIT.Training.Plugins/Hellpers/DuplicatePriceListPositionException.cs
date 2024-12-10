using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwaraIT.Kuralbek.Plugins.Hellpers
{
    public class DuplicatePriceListPositionException : Exception
    {
        public int ErrorCode { get; }

        public DuplicatePriceListPositionException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }

}
