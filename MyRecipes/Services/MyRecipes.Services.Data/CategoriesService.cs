﻿namespace MyRecipes.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using MyRecipes.Data.Common.Repositories;

    using MyRecipes.Data.Models;

    public class CategoriesService : ICategoriesService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;

        public CategoriesService(IDeletableEntityRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public IEnumerable<KeyValuePair<string, string>> GetAllKeyValuePairs()
       {
            var repositories = this.categoriesRepository.AllAsNoTracking()
                                                         .Select(x => new
                                                         {
                                                             x.Id,
                                                             x.Name,
                                                         })
                                                         .OrderBy(x => x.Name)
                                                         .ToList()
                                                         .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name));
            return repositories;
       }
    }
}
