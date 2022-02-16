using RestaurantApi.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace RestaurantApi.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        RestaurantDto GetById(int id);
        public void Delete(int id);
        public void Update(int id, UpdateRestaurantDto dto);
    }
}