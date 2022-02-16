using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Entities;
using RestaurantApi.Exceptions;
using RestaurantApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Services
{
    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public int CreateDish(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            if (restaurant == null) throw new NotFoundException("Restaurant not found");

            var dishEntity = _mapper.Map<Dish>(dto);
            dishEntity.RestaurantId = restaurant.Id;

            _context.Dishes.Add(dishEntity);
            _context.SaveChanges();

            return dishEntity.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = _context.Dishes.FirstOrDefault(x => x.Id == dishId);

            if (dish == null || dish.RestaurantId != dish.RestaurantId) throw new NotFoundException("Dish not found");

            var dishDto = _mapper.Map<DishDto>(dish);

            return dishDto;
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes); // nie musi byc mapy na listy, wystarczy ze podamy ze to lista i to lista

            return dishDtos;
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();
        }

        public Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context.Restaurants
               .Include(x => x.Dishes)
               .FirstOrDefault(r => r.Id == restaurantId);

            if (restaurant == null) throw new NotFoundException("Restaurant not found");

            return restaurant;
        }
    }
}
