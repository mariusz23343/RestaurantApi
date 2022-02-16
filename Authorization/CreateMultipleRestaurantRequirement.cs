using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Authorization
{
    public class CreateMultipleRestaurantRequirement : IAuthorizationRequirement
    {
        public CreateMultipleRestaurantRequirement(int minimumRestaurantCreated)
        {
            MinimumRestaurantCreated = minimumRestaurantCreated;
        }

        public int MinimumRestaurantCreated { get; }
    }
}
