using RestaurantApi.Models;
using System.Collections.Generic;

namespace RestaurantApi.Services
{
    public interface IDishService
    {
        int CreateDish(int restaurantId, CreateDishDto dto);
        public DishDto GetById(int restaurantId, int dishId);
        public List<DishDto> GetAll(int restaurantId);
        void RemoveAll(int restaurantId);
    }
}