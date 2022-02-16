using Microsoft.AspNetCore.Authorization;
using RestaurantApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantApi.Authorization
{
    public class CreatedMultipleRestaurantRequirementHandler : AuthorizationHandler<CreateMultipleRestaurantRequirement>
    {
        private readonly RestaurantDbContext _context;

        public CreatedMultipleRestaurantRequirementHandler(RestaurantDbContext context)
        {
            _context = context;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreateMultipleRestaurantRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var createdRestaurantsCount = _context.Restaurants.Count(r => r.CreatedById == userId);

            if (createdRestaurantsCount >= requirement.MinimumRestaurantCreated)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
