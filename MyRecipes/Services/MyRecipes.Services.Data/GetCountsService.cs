﻿using MyRecipes.Data.Common.Repositories;
using MyRecipes.Data.Models;
using MyRecipes.Services.Data.Models;
using MyRecipes.Web.ViewModels.Home;
using System.Linq;

namespace MyRecipes.Services.Data
{

    public class GetCountsService : IGetCountsService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;
        private readonly IRepository<Image> imagesRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;
        private readonly IDeletableEntityRepository<Recipe> recipesRepository;

        public GetCountsService(
            IDeletableEntityRepository<Category> categoriesRepository,
            IRepository<Image> imagesRepository,
            IDeletableEntityRepository<Ingredient> ingredientsRepository,
            IDeletableEntityRepository<Recipe> recipesRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.imagesRepository = imagesRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.recipesRepository = recipesRepository;
        }

        public CountsDto GetCounts()
        {
            var model = new CountsDto
            {
                IngredientsCount = this.ingredientsRepository.All().Count(),
                CategoriesCount = this.categoriesRepository.All().Count(),
                ImagesCount = this.imagesRepository.All().Count(),
                RecipesCount = this.recipesRepository.All().Count(),
            };

            return model;
        }
    }
}
