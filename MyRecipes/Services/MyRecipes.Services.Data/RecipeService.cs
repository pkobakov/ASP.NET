﻿namespace MyRecipes.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MyRecipes.Data.Common.Repositories;
    using MyRecipes.Data.Models;
    using MyRecipes.Services.Mapping;
    using MyRecipes.Web.ViewModels.Recipes;

    public class RecipeService : IRecipeService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "jpeg", "png", "gif" };
        private readonly IDeletableEntityRepository<Recipe> recipesRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientRepository;

        public RecipeService(
            IDeletableEntityRepository<Recipe> recipesRepository,
            IDeletableEntityRepository<Ingredient> ingredientRepository)
        {
            this.recipesRepository = recipesRepository;
            this.ingredientRepository = ingredientRepository;
        }

        public async Task CreateAsync(CreateRecipeInputModel model, string userId, string imagePath)
        {
           var recipe = new Recipe
           {
            Name = model.Name,
            CookingTime = TimeSpan.FromMinutes(model.CookingTime),
            PreparationTime = TimeSpan.FromMinutes(model.PreparationTime),
            Instructions = model.Instructions,
            PortionsCount = model.PortionsCount,
            CategoryId = model.CategoryId,
            AddedByUserId = userId,
           };

           foreach (var inputIngredient in model.Ingredients)
           {
                var ingredient = this.ingredientRepository.All().FirstOrDefault(x => x.Name == inputIngredient.Name);

                if (ingredient == null)
                {
                    ingredient = new Ingredient { Name = inputIngredient.Name };
                }

                recipe.Ingredients.Add(new RecipeIngredient
                {
                    Ingredient = ingredient,
                    Quantity = inputIngredient.Quantity,
                });
           }

           Directory.CreateDirectory($"{imagePath}/recipes/");

           foreach (var image in model.Images)
           {
                var extension = Path.GetExtension(image.FileName).TrimStart('.');

                if (!this.allowedExtensions.Any(x => extension.EndsWith(x)))
                {
                    throw new Exception($"Invalid image extension {extension}");
                }

                var dbImage = new Image
                {
                  AddedByUserId = userId,
                  Extension = extension,
                };

                recipe.Images.Add(dbImage);

                var physicalPath = $"{imagePath}/recipes/{dbImage.Id}.{extension}";
                using Stream fileStream = new FileStream(physicalPath, FileMode.Create);
                await image.CopyToAsync(fileStream);
           }

           await this.recipesRepository.AddAsync(recipe);
           await this.recipesRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var recipe = this.recipesRepository.All().FirstOrDefault(x => x.Id == id);
            this.recipesRepository.Delete(recipe);
            await this.recipesRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll<T>(int page, int itemsPerPage = 4)
        {
            var recipes = this.recipesRepository
                              .AllAsNoTracking()
                              .OrderByDescending(x => x.Id)
                              .Skip((page - 1) * itemsPerPage)
                              .Take(itemsPerPage)
                              .To<T>()
                              .ToList();

            return recipes;
        }

        public T GetById<T>(int id)
        {
            var singleRecipe = this.recipesRepository
                                   .AllAsNoTracking()
                                   .Where(x => x.Id == id)
                                   .To<T>()
                                   .FirstOrDefault();
            return singleRecipe;
        }

        public IEnumerable<T> GetByIngredients<T>(IEnumerable<int> ingredientsIds)
        {
            var query = this.recipesRepository.All().AsQueryable();
            foreach (var ingredientId in ingredientsIds)
            {
                query = query.Where(x => x.Ingredients.Any(i => i.IngredientId == ingredientId));
            }

            return query.To<T>().ToList();
        }

        public int GetCount()
        {
            return this.recipesRepository.All().Count();
        }

        public IEnumerable<T> GetRandom<T>(int count)
        {
            return this.recipesRepository.All()
                .OrderBy(x => Guid.NewGuid())
                .To<T>()
                .Take(count)
                .ToList();
        }

        public async Task UpdateAsync(int id, EditRecipeInputModel model)
        {
            var recipe = this.recipesRepository.All().FirstOrDefault(x => x.Id == id);
            recipe.Name = model.Name;
            recipe.Instructions = model.Instructions;
            recipe.PreparationTime = TimeSpan.FromMinutes(model.PreparationTime);
            recipe.CookingTime = TimeSpan.FromMinutes(model.CookingTime);
            recipe.PortionsCount = model.PortionsCount;
            recipe.CategoryId = model.CategoryId;

            await this.recipesRepository.SaveChangesAsync();
        }
    }
}
