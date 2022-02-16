using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Exceptions
{
    public class BadRequestEception : Exception
    {
        public BadRequestEception(string message) : base(message)
        {

        }
    }
}
